using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApi.DTO.Request;
using TodoApi.DTO.Response;
using TodoApi.Services;

namespace TodoApi.Controllers;

[Authorize]
[ApiController]
[Route("api/todos")]
public class TodoController : ControllerBase
{
    private readonly TodoService _service;
    private readonly ILogger<TodoController> _logger;

    public TodoController(TodoService service, ILogger<TodoController> logger)
    {
        _service = service;
        _logger = logger;
    }

    private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

    [HttpGet]
    public async Task<IActionResult> GetTodos() =>
        Ok(new ApiResponse<object>
        {
            Success = true,
            Data = await _service.GetUserTodosAsync(GetUserId())
        });

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTodoById(string id)
    {
        var todo = await _service.GetTodoByIdAsync(id, GetUserId());

        if (todo == null)
        {
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = "This have been delete or it not your"
            });
        }

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Data = todo
        });
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateTodo([FromBody] CreateTodoRequest request) =>
        Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Todo created",
            Data = await _service.CreateTodoAsync(GetUserId(), request)
        });

    [HttpPost("{id}/update-content")]
    public async Task<IActionResult> UpdateTodoContent(string id, [FromBody] UpdateTodoRequest request)
    {
        var todo = await _service.UpdateTodoAsync(id, GetUserId(), request);
        if (todo == null)
            return NotFound(new ApiResponse<object> { Success = false, Message = "Todo not found" });

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Todo updated",
            Data = todo
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodo(string id)
    {
        var success = await _service.DeleteTodoAsync(id, GetUserId());
        if (!success)
            return NotFound(new ApiResponse<object> { Success = false, Message = "Todo not found" });

        return Ok(new ApiResponse<object> { Success = true, Message = "Todo deleted" });
    }
}
