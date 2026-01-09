using TodoApi.DTO.Request;
using TodoApi.Models;
using TodoApi.Repositories;

namespace TodoApi.Services;

public class TodoService
{
    private readonly TodoRepository _repo;

    public TodoService(TodoRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<Todo>> GetUserTodosAsync(string userId)
    {
        return await _repo.GetByUserIdAsync(userId);
    }

    public async Task<Todo?> GetTodoByIdAsync(string id, string userId)
    {
        var todo = await _repo.GetByIdAsync(id);

        if (todo == null || todo.UserId != userId)
            return null;

        return todo;
    }

    public async Task<Todo> CreateTodoAsync(string userId, CreateTodoRequest request)
    {
        var todo = new Todo
        {
            UserId = userId,
            Title = request.Title,
            Description = request.Description,
            IsCompleted = false
        };

        await _repo.CreateAsync(todo);
        return todo;
    }

    public async Task<Todo?> UpdateTodoAsync(string id, string userId, UpdateTodoRequest request)
    {
        var todo = await _repo.GetByIdAsync(id);

        if (todo == null || todo.UserId != userId) return null;

        todo.Title = request.Title;
        todo.Description = request.Description;
        todo.IsCompleted = request.IsCompleted;

        await _repo.UpdateAsync(id, todo);
        return todo;
    }

    public async Task<bool> DeleteTodoAsync(string id, string userId)
    {
        var todo = await _repo.GetByIdAsync(id);

        if (todo == null || todo.UserId != userId) return false;

        await _repo.DeleteAsync(id);
        return true;
    }
}