using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services;

public class TokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(User user)
    {
        // 1. Создаем список "утверждений" (claims) - это информация о пользователе в токене
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Guid.ToString()), // Уникальный ID пользователя
            new Claim(ClaimTypes.Name, user.Login),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Уникальный ID самого токена
        };

        // 2. Добавляем роль "Admin", если пользователь является админом.
        if (user.Admin)
        {
            claims.Add(new Claim(ClaimTypes.Role, "Admin"));
        }

        // 3. Получаем секретный ключ из конфигурации
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        
        // 4. Создаем учетные данные для подписи токена
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        // 5. Устанавливаем время жизни токена
        var expires = DateTime.UtcNow.AddDays(7); // Например, 7 дней

        // 6. Создаем сам токен
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        // 7. Сериализуем токен в строку
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}