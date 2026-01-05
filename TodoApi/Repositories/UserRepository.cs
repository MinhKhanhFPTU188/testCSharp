using MongoDB.Driver;
using TodoApi.Models;

namespace TodoApi.Repositories;

public class UserRepository
{
    private readonly IMongoCollection<User> _users;

    public UserRepository(MongoDbContext context)
    {
        _users = context.Users;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
    }

    public Task CreateAsync(User user) =>
        _users.InsertOneAsync(user);
}
