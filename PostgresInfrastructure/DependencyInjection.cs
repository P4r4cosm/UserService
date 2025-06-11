using Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PostgresInfrastructure.Persistence;
using PostgresInfrastructure.Repositories;

namespace PostgresInfrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddPostgresInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Получаем строку подключения из конфигурации
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // Регистрируем DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));
        // Регистрируем репозиторий
        services.AddScoped<IUserRepository, PostgresUserRepository>();
        
        return services;
    }
}