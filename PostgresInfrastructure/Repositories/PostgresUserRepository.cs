using Domain.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using PostgresInfrastructure.Persistence;

namespace PostgresInfrastructure.Repositories;

public class PostgresUserRepository: IUserRepository
{
    private readonly ApplicationDbContext _dbContext;
    public PostgresUserRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    
    public async Task<User?> GetByLoginAsync(string login)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u=>u.Login == login);
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u=>u.Guid == id);
    }

    public async Task<User?> GetByLoginAndPasswordAsync(string login, string password)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u=>u.Login == login);
        if (user == null) 
            return null;
        if (user.VerifyPassword(password))
            return user;
        return null;
    }

    public async Task<IEnumerable<User>> GetAllActiveAsync()
    {
        return await _dbContext.Users.Where(u=>u.RevokedOn == null).OrderBy(u=>u.CreatedOn).ToListAsync();
    }

    public async Task<IEnumerable<User>> GetUsersOlderThanAsync(int age)
    {
        var cutoffDate = DateTime.UtcNow.AddYears(-age);
        return await _dbContext.Users
            .Where(u => u.RevokedOn==null && u.Birthday.HasValue && u.Birthday.Value <= cutoffDate)
            .OrderBy(u => u.CreatedOn)
            .ToListAsync();
    }

    public async Task<bool> IsLoginUniqueAsync(string login)
    {
        return !await _dbContext.Users.AnyAsync(u => u.Login == login);
    }

    public async Task AddAsync(User user)
    {
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync(); 
    }

    public async Task UpdateAsync(User user)
    {
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(User user)
    {
        _dbContext.Remove(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(User user, string actorLogin)
    {
        user.SoftDelete(actorLogin);
        await UpdateAsync(user);
    }

    public async Task RestoreAsync(User user, string actorLogin)
    {
        user.Restore();
        await UpdateAsync(user);
    }
}