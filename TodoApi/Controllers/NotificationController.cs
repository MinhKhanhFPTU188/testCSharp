using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Services;

namespace TodoApi.Controllers;

[Authorize]
[ApiController]
[Route("api/notifications")]
public class NotificationController : ControllerBase
{
    private readonly NotificationService _notificationService;

    public NotificationController(NotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet("subscribe")]
    public async Task Subscribe(CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            Response.StatusCode = 401;
            return;
        }

        Response.Headers.Append("Content-Type", "text/event-stream");
        Response.Headers.Append("Cache-Control", "no-cache");
        Response.Headers.Append("Connection", "keep-alive");

        var reader = _notificationService.Subscribe(userId);

        try
        {
            // Send initial connection message
            await Response.WriteAsync($"data: {{\"type\":\"CONNECTED\", \"message\":\"Connected to notification stream\"}}\n\n", cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);

            // Keep reading from the channel until client disconnects
            await foreach (var message in reader.ReadAllAsync(cancellationToken))
            {
                await Response.WriteAsync(message, cancellationToken);
                await Response.Body.FlushAsync(cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            // Client disconnected
        }
        finally
        {
            _notificationService.Unsubscribe(userId, reader);
        }
    }
}