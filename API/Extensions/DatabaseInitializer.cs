using Microsoft.EntityFrameworkCore;
using PostgresInfrastructure.Persistence;

public static class DatabaseInitializer
{
    /// <summary>
    /// Применяет все ожидающие миграции к базе данных.
    /// Этот метод нужно вызывать при старте приложения.
    /// </summary>
    /// <param name="app">Экземпляр IApplicationBuilder для получения сервисов.</param>
    public static async Task InitializeDatabaseAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        
        var services = scope.ServiceProvider;
        var loggerFactory = services.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("DatabaseInitializer"); // Создаем логгер с категорией "DatabaseInitializer"
        
        try
        {
            logger.LogInformation("Starting database initialization...");

            // 1. Получаем DbContext
            var dbContext = services.GetRequiredService<ApplicationDbContext>();

            // 2. Применяем миграции
            logger.LogInformation("Applying database migrations...");
            await dbContext.Database.MigrateAsync();
            logger.LogInformation("Database migrations applied successfully.");

            // 3. Вызываем наш Seeder для наполнения данными
            var seeder = services.GetRequiredService<DatabaseSeeder>();
            logger.LogInformation("Seeding initial data...");
            await seeder.SeedAsync();
            logger.LogInformation("Initial data seeded successfully.");
            
            logger.LogInformation("Database initialization completed.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during database initialization.");
            throw; // Перевыбрасываем исключение
        }
    }
}