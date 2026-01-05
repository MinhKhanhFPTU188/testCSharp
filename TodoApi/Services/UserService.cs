using BCrypt.Net;
using Microsoft.AspNetCore.Identity.Data;
using TodoApi.DTO.Request;
using TodoApi.Models;
using TodoApi.Repositories;

namespace TodoApi.Services;

public class UserService
{
    private readonly UserRepository _repo;

    public UserService(UserRepository repo)
    {
        _repo = repo;
    }

    public async Task RegisterAsync(RegisterUserRequest request)
    {
        var existing = await _repo.GetByEmailAsync(request.Email);
        if (existing != null)
            throw new Exception("Email already exists");

        var user = new User
        {
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        await _repo.CreateAsync(user);
    }

    public async Task<User?> LoginAsync(LoginUserRequest request)
    {
        var user = await _repo.GetByEmailAsync(request.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return null; 
        }
        return user; 
    }

}
