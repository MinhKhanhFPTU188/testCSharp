using Microsoft.AspNetCore.Mvc;
using TodoApi.DTO.Request;
using TodoApi.Services;

namespace TodoApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserService _service;

    public AuthController(UserService service)
    {
        _service = service;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        try
        {
            await _service.RegisterAsync(request);
            return Ok(new { message = "User registered successfully" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
    {
        var user = await _service.LoginAsync(request);

        if (user == null)
        {
            return Unauthorized(new { message = "Invalid email or password" });
        }

        // For now, we just return a success message.
        // In a real app, you would return a JWT Token here.
        return Ok(new { message = "Login successful", userId = user.Id, email = user.Email });
    }
}
