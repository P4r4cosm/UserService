using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace PostgresInfrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<User> Users { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        // Login должен быть уникальным
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Login)
            .IsUnique();
        
        // Указываем, что Guid генерируется базой данных
        modelBuilder.Entity<User>()
            .Property(u => u.Guid)
            .ValueGeneratedOnAdd();
            
        base.OnModelCreating(modelBuilder);
    }
}