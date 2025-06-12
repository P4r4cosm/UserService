using System.Text;
using API.Middleware;
using Application.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PostgresInfrastructure;
using PostgresInfrastructure.Persistence;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddPostgresInfrastructure(builder.Configuration);
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<TokenService>();

// Настройка JWT-аутентификации 
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Валидировать ключ безопасности
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),

            // Валидировать издателя токена
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],

            // Валидировать потребителя токена
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],

            // Валидировать время жизни токена
            ValidateLifetime = true,

            // Установка нулевого ClockSkew, чтобы токены истекали точно в указанное время
            ClockSkew = TimeSpan.Zero
        };

    });

//  Настройка Swagger для поддержки JWT
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "User API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
        In = ParameterLocation.Header, 
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        { 
            new OpenApiSecurityScheme { 
                Reference = new OpenApiReference { 
                    Type = ReferenceType.SecurityScheme, Id = "Bearer" 
                } 
            },
            Array.Empty<string>()
        } 
    });
});

var app = builder.Build();

//применяем миграции
await app.InitializeDatabaseAsync();

app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "UserService V1");
    options.RoutePrefix = string.Empty; 
});
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

async Task SeedDatabaseAsync(IHost host)
{
    using (var scope = host.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var seeder = services.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedAsync();
    }
}