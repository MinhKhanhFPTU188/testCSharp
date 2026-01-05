using MongoDB.Driver;
using Microsoft.Extensions.Options;
using TodoApi.Models;

namespace TodoApi.Repositories;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);
    }

    public IMongoCollection<User> Users =>
        _database.GetCollection<User>("Users");

    public IMongoCollection<Todo> Todos =>
        _database.GetCollection<Todo>("Todos");
}
