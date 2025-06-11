using Domain.Abstractions;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace PostgresInfrastructure.Persistence;

public class DatabaseSeeder
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<DatabaseSeeder> _logger;
    public DatabaseSeeder(IUserRepository userRepository, ILogger<DatabaseSeeder> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }
    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAdminUserAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during database seeding.");
        }
    }
    private async Task TrySeedAdminUserAsync()
    {
        if (await _userRepository.GetByLoginAsync("admin") == null)
        {
            _logger.LogInformation("Admin user not found, creating one...");

            var adminUser = new User(
                login: "admin",
                password: "password", // Пароль должен соответствовать правилам домена
                name: "administrator",
                gender: Gender.Unknown, 
                birthday: null,
                admin: true,
                createdBy: "System"
            );

            await _userRepository.AddAsync(adminUser);
            _logger.LogInformation("Admin user created successfully.");
        }
    }
}