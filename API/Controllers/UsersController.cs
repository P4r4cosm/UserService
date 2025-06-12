using System.Security.Claims;
using Application.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Требует валидный токен для методов в этом контроллере
public class UsersController : ControllerBase
{
    private readonly UserService _userService;

    // Приватное свойство для удобного и безопасного получения логина текущего пользователя из токена.
    private string CurrentUserLogin => HttpContext.User.FindFirstValue(ClaimTypes.Name);

    // Конструктор теперь не требует ILogger
    public UsersController(UserService userService)
    {
        _userService = userService;
    }

    // 1. Создание пользователя
    [HttpPost("create")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto request)
    {
        var createdUser = await _userService.CreateUserAsync(request, CurrentUserLogin);
        return CreatedAtAction(nameof(GetUserByLogin), new { login = createdUser.Login }, createdUser);
    }

    // 2. Изменение имени, пола или даты рождения
    [HttpPut("{loginToUpdate}/info")]
    public async Task<IActionResult> UpdateUserInfo(string loginToUpdate, [FromBody] UpdateUserInfoDto dto)
    {
        await _userService.UpdateUserInfoAsync(loginToUpdate, dto, CurrentUserLogin);
        return NoContent();
    }

    // 3. Изменение пароля
    [HttpPut("{loginToUpdate}/password")]
    public async Task<IActionResult> UpdateUserPassword(string loginToUpdate, [FromBody] UpdatePasswordDto dto)
    {
        await _userService.UpdatePasswordAsync(loginToUpdate, dto.NewPassword, CurrentUserLogin);
        return NoContent();
    }

    // 4. Изменение логина
    [HttpPut("{loginToUpdate}/login")]
    public async Task<IActionResult> UpdateUserLogin(string loginToUpdate, [FromBody] UpdateLoginDto dto)
    {
        await _userService.UpdateLoginAsync(loginToUpdate, dto.NewLogin, CurrentUserLogin);
        return NoContent();
    }

    // 5. Запрос списка всех активных пользователей
    [HttpGet("active")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetActiveUsers()
    {
        var users = await _userService.GetAllActiveUsersAsync();
        return Ok(users);
    }

    // 6. Запрос пользователя по логину (доступно админам)
    [HttpGet("{login}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUserByLogin(string login)
    {
        var user = await _userService.GetUserByLoginAsync(login);
        return Ok(user);
    }

    // 7. Запрос данных о себе
    [HttpPost("me/profile-data")] 
    public async Task<IActionResult> GetMyProfileWithPassword([FromBody] PasswordConfirmationDto dto)
    {
        var user = await _userService.GetUserByLoginAndPasswordAsync(CurrentUserLogin, dto.Password, CurrentUserLogin);
        return Ok(user);
    }

    // 8. Запрос всех пользователей старше определённого возраста
    [HttpGet("older-than/{age}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUsersOlderThan(int age)
    {
        var users = await _userService.GetUsersOlderThanAsync(age);
        return Ok(users);
    }

    // 9. Мягкое удаление пользователя
    [HttpDelete("{login}/soft-delete")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SoftDeleteUser(string login)
    {
        await _userService.SoftDeleteUserAsync(login, CurrentUserLogin);
        return NoContent();
    }

    // 10. Полное удаление пользователя
    [HttpDelete("{login}/hard-delete")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> HardDeleteUser(string login)
    {
        await _userService.HardDeleteUserAsync(login, CurrentUserLogin);
        return NoContent();
    }
    
    // 11. Восстановление пользователя
    [HttpPut("{login}/restore")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RestoreUser(string login)
    {
        await _userService.RestoreUserAsync(login, CurrentUserLogin);
        return NoContent();
    }
}