using Domain.Entities;

namespace Domain.Abstractions;

public interface IUserRepository
{
    Task<User?> GetByLoginAsync(string login);
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByLoginAndPasswordAsync(string login, string password);
    Task<IEnumerable<User>> GetAllActiveAsync(); // Для п.5 ТЗ
    Task<IEnumerable<User>> GetUsersOlderThanAsync(int age); // Для п.8 ТЗ
    Task<bool> IsLoginUniqueAsync(string login);

    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(User user); // Для полного удаления
    Task SoftDeleteAsync(User user, string actorLogin);
    
    Task RestoreAsync(User user, string actorLogin);
}