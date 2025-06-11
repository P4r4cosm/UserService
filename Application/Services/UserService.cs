using Application.Dtos;
using Application.Exceptions;
using Domain.Abstractions;
using Domain.Entities;
using UnauthorizedAccessException = Application.Exceptions.UnauthorizedAccessException;

public class UserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    // 1. Создание пользователя (доступно Админам)
    public async Task<UserDto> CreateUserAsync(CreateUserDto dto, string createdByLogin)
    {
        // Проверка прав (роль "Admin") происходит в контроллере
        if (!await _userRepository.IsLoginUniqueAsync(dto.Login))
        {
            throw new ValidationException($"Login '{dto.Login}' is already taken.");
        }

        var user = new User(dto.Login, dto.Password, dto.Name, dto.Gender, dto.Birthday, dto.IsAdmin, createdByLogin);
        await _userRepository.AddAsync(user);
        
        return MapToUserDto(user);
    }

    // 2. Изменение имени, пола или даты рождения
    public async Task UpdateUserInfoAsync(string loginToUpdate, UpdateUserInfoDto dto, string actorLogin)
    {
        var (_, userToUpdate) = await AuthorizeUserModificationAsync(actorLogin, loginToUpdate);
        
        userToUpdate.UpdateInfo(dto.Name, dto.Gender, dto.Birthday, actorLogin);
        await _userRepository.UpdateAsync(userToUpdate);
    }

    // 3. Изменение пароля
    public async Task UpdatePasswordAsync(string loginToUpdate, string newPassword, string actorLogin)
    {
        var (_, userToUpdate) = await AuthorizeUserModificationAsync(actorLogin, loginToUpdate);

        userToUpdate.ChangePassword(newPassword, actorLogin);
        await _userRepository.UpdateAsync(userToUpdate);
    }

    // 4. Изменение логина
    public async Task UpdateLoginAsync(string loginToUpdate, string newLogin, string actorLogin)
    {
        if (!await _userRepository.IsLoginUniqueAsync(newLogin))
        {
            throw new ValidationException("Login is already taken");
        }

        var (_, userToUpdate) = await AuthorizeUserModificationAsync(actorLogin, loginToUpdate);
        
        userToUpdate.UpdateLogin(newLogin, actorLogin);
        await _userRepository.UpdateAsync(userToUpdate);
    }

    // 5. Запрос списка всех активных пользователей
    public async Task<IEnumerable<UserDto>> GetAllActiveUsersAsync()
    {
        var users = await _userRepository.GetAllActiveAsync();
        return users.Select(MapToUserDto);
    }

    // 6. Запрос пользователя по логину
    public async Task<UserDto> GetUserByLoginAsync(string login)
    {
        var user = await _userRepository.GetByLoginAsync(login) 
                   ?? throw new NotFoundException("User not found", "Login");
        
        return MapToUserDto(user);
    }

    // 7. Запрос пользователя по логину и паролю
    public async Task<UserDto> GetUserByLoginAndPasswordAsync(string login, string password, string actorLogin)
    {
        if (actorLogin != login)
        {
            throw new UnauthorizedAccessException("You can only request your own data.");
        }

        var user = await _userRepository.GetByLoginAsync(login);
        if (user == null || !user.IsActive || !user.VerifyPassword(password)) 
        {
            throw new UnauthorizedAccessException("Invalid credentials or inactive user.");
        }
        
        return MapToUserDto(user);
    }

    // 8. Запрос всех пользователей старше определённого возраста
    public async Task<IEnumerable<UserDto>> GetUsersOlderThanAsync(int age)
    {
        var users = await _userRepository.GetUsersOlderThanAsync(age);
        return users.Select(MapToUserDto);
    }

    // 9. Мягкое удаление пользователя
    public async Task SoftDeleteUserAsync(string loginToDelete, string actorLogin)
    {
        var userToDelete = await AuthorizeAdminActionAsync(actorLogin, loginToDelete);
        await _userRepository.SoftDeleteAsync(userToDelete, actorLogin);
    }

    // 9. Полное удаление пользователя
    public async Task HardDeleteUserAsync(string loginToDelete, string actorLogin)
    {
        var userToDelete = await AuthorizeAdminActionAsync(actorLogin, loginToDelete);
        await _userRepository.DeleteAsync(userToDelete);
    }

    // 10. Восстановление пользователя
    public async Task RestoreUserAsync(string loginToRestore, string actorLogin)
    {
        var userToRestore = await AuthorizeAdminActionAsync(actorLogin, loginToRestore);
        await _userRepository.RestoreAsync(userToRestore, actorLogin);
    }

    #region Private Helpers

    // Помощник для операций, которые может делать админ ИЛИ сам пользователь
    private async Task<(User actor, User targetUser)> AuthorizeUserModificationAsync(string actorLogin, string targetLogin)
    {
        var actor = await _userRepository.GetByLoginAsync(actorLogin) 
                    ?? throw new NotFoundException("Requesting user (actor) not found.", "actorLogin");
        
        var targetUser = await _userRepository.GetByLoginAsync(targetLogin) 
                         ?? throw new NotFoundException("Target user to modify not found.", "targetLogin");

        bool canModify = actor.Admin || (actor.Login == targetUser.Login && targetUser.IsActive);
        if (!canModify)
        {
            throw new UnauthorizedAccessException("You do not have permission to modify this user.");
        }
        
        return (actor, targetUser);
    }
    
    // Помощник для операций, которые может делать ТОЛЬКО админ
    private async Task<User> AuthorizeAdminActionAsync(string actorLogin, string targetLogin)
    {
        var actor = await _userRepository.GetByLoginAsync(actorLogin) 
                    ?? throw new NotFoundException("Requesting user (actor) not found.", "actorLogin");

        if (!actor.Admin) 
        {
            throw new UnauthorizedAccessException("This action can only be performed by an administrator.");
        }
        
        var targetUser = await _userRepository.GetByLoginAsync(targetLogin)
                         ?? throw new NotFoundException("Target user not found.", "targetLogin");

        return targetUser;
    }

    // Помощник для маппинга User -> UserDto
    private UserDto MapToUserDto(User user)
    {
        return new UserDto
        {
            Guid = user.Guid,
            Login = user.Login,
            Name = user.Name,
            Gender = user.Gender,
            Birthday = user.Birthday,
            IsAdmin = user.Admin,
            IsActive = user.IsActive,
            CreatedOn = user.CreatedOn
        };
    }

    #endregion
}