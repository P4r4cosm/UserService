using Application.Dtos;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController: ControllerBase
{
    private readonly UserService _userService;
    private readonly TokenService _tokenService;

    public AuthController(UserService userService, TokenService tokenService)
    {
        _userService = userService;
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        // Используем метод аутентификации из UserService
        var user = await _userService.AuthenticateAsync(request.Login, request.Password);

        if (user == null)
        {
            return Unauthorized("Invalid login or password.");
        }

        var token = _tokenService.GenerateToken(user);
        
        // Возвращаем токен клиенту
        return Ok(new { Token = token });
    }

}
