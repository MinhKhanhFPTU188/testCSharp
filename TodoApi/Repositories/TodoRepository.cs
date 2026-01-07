using MongoDB.Driver;
using TodoApi.Models;

namespace TodoApi.Repositories;

public class TodoRepository
{
    private readonly IMongoCollection<Todo> _todos;

    public TodoRepository(MongoDbContext context)
    {
        _todos = context.Todos;
    }

    public async Task<List<Todo>> GetByUserIdAsync(string userId)
    {
        return await _todos.Find(t => t.UserId == userId).ToListAsync();
    }

    public async Task<Todo?> GetByIdAsync(string id)
    {
        return await _todos.Find(t => t.Id == id).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(Todo todo)
    {
        await _todos.InsertOneAsync(todo);
    }

    public async Task UpdateAsync(string id, Todo todo)
    {
        await _todos.ReplaceOneAsync(t => t.Id == id, todo);
    }

    public async Task DeleteAsync(string id)
    {
        await _todos.DeleteOneAsync(t => t.Id == id);
    }
}