using Microsoft.AspNetCore.Mvc;
using TodoApi.DTO.Request;
using TodoApi.DTO.Response;
using TodoApi.Services;

namespace TodoApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserService _service;
    private readonly JwtService _jwt;
    private readonly ILogger<AuthController> _logger;

    public AuthController(UserService service, JwtService jwt, ILogger<AuthController> logger)
    {
        _service = service;
        _jwt = jwt;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        try
        {
            await _service.RegisterAsync(request);
            return Ok(new ApiResponse<AuthPayload>
            {
                Success = true,
                Message = "Register successful"
            });
        }
        catch (Exception ex) 
        {
            _logger.LogError(ex, "Register failed for email: {Email}", request.Email); 

            return BadRequest(new ApiResponse<AuthPayload>
            {
                Success = false,
                Message = "Registration failed",
                Errors = ex.Message 
            });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
    {
        try
        {
            var user = await _service.LoginAsync(request);
            if (user == null)
            {
                return Unauthorized(new ApiResponse<AuthPayload>
                {
                    Success = false,
                    Message = "Invalid email or password"
                });
            }

            var (token, expiresAt) = _jwt.GenerateToken(user);

            return Ok(new ApiResponse<AuthPayload>
            {
                Success = true,
                Message = "Login successful",
                Data = new AuthPayload
                {
                    UserId = user.Id!,
                    Email = user.Email,
                    AccessToken = token,
                    ExpiresAt = expiresAt
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login failed for email: {Email}", request.Email);
            return StatusCode(500, new ApiResponse<AuthPayload> 
            { 
                Success = false, 
                Message = "An internal error occurred" 
            });
        }
    }
}