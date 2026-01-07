using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApi.DTO.Request;
using TodoApi.DTO.Response;
using TodoApi.Services;

namespace TodoApi.Controllers;

[Authorize] // Requires valid JWT Token
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
    public async Task<IActionResult> GetTodos()
    {
        var todos = await _service.GetUserTodosAsync(GetUserId());
        return Ok(new ApiResponse<object>
        {
            Success = true,
            Data = todos
        });
    }

    [HttpPost]
    public async Task<IActionResult> CreateTodo([FromBody] CreateTodoRequest request)
    {
        var todo = await _service.CreateTodoAsync(GetUserId(), request);
        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Todo created",
            Data = todo
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTodo(string id, [FromBody] UpdateTodoRequest request)
    {
        var updatedTodo = await _service.UpdateTodoAsync(id, GetUserId(), request);
        
        if (updatedTodo == null)
            return NotFound(new ApiResponse<object> { Success = false, Message = "Todo not found" });

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Todo updated",
            Data = updatedTodo
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