using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Domain.Entities;

using BCrypt.Net;

public class User
{
    [Key]
    public Guid Guid { get; private set; }
    public string Login { get; private set; } //Уникальный Логин (запрещены все символы кроме латинских букв и цифр)
    public string Password { get; private set; } // Пароль(запрещены все символы кроме латинских букв и цифр) (хэшируется)
    public string Name { get; private set; } //Имя (запрещены все символы кроме латинских и русских букв)
    public Gender Gender { get; private set; } //0 - женщина, 1 - мужчина, 2 - неизвестно
    public DateTime? Birthday { get; private set; }
    public bool Admin { get; private set; } //Указание - является ли пользователь админом
    public DateTime CreatedOn { get; private set; } //Дата создания пользователя
    public string CreatedBy { get; private set; } // Пользователя, от имени которого этот пользователь создан
    public DateTime? ModifiedOn { get; private set; } //Дата изменения пользователя
    public string? ModifiedBy { get; private set; } //Логин Пользователя, от имени которого этот пользователь изменён
    public DateTime? RevokedOn { get; private set; } // Дата удаления пользователя
    public string? RevokedBy { get; private set; } //Логин Пользователя, от имени которого этот пользователь удалён


    public User(string login, string password, string name, Gender gender, DateTime? birthday, bool admin,
        string createdBy)
    {
        ValidateLogin(login);
        ValidatePassword(password);
        ValidateName(name);
        Guid = Guid.NewGuid();
        Login = login;
        Password = HashPassword(password);
        Name = name;
        Gender = gender;
        Birthday = birthday;
        Admin = admin;
        CreatedOn = DateTime.UtcNow;
        CreatedBy = createdBy;
    }
    // Для использования EF Core
    private User()
    {
        // Не вызываем валидацию, так как объект создаётся из БД
    }

    public bool IsActive => RevokedOn == null;

    //Изменение имени, пола или даты рождения пользователя
    //(Может менять Администратор, либо лично пользователь, если он активен (отсутствует RevokedOn))
    public void UpdateInfo(string newName, Gender newGender, DateTime? newBirthday, string modifiedBy)
    {
        ValidateName(newName);
        Name = newName;
        Gender = newGender;
        Birthday = newBirthday;
        ModifiedOn = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    //Изменение пароля (Пароль может менять либо Администратор, либо лично пользователь, если он активен (отсутствует RevokedOn))
    public void ChangePassword(string newPassword, string modifiedBy)
    {
        ValidatePassword(newPassword);
        Password = HashPassword(newPassword);
        ModifiedOn = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    //Изменение логина (Логин может менять либо Администратор, либо лично пользователь,
    //если он активен (отсутствует RevokedOn), логин должен оставаться уникальным)
    public void UpdateLogin(string newLogin, string modifiedBy)
    {
        ValidateLogin(newLogin);
        Login = newLogin;
        ModifiedOn = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }
    
    //мягкое удаление
    public void SoftDelete(string revokedBy)
    {
        RevokedOn = DateTime.UtcNow;
        RevokedBy = revokedBy;
    }
    
    //восстановление
    public void Restore()
    {
        RevokedOn = null;
        RevokedBy = null;
    }

    // --- Статические методы валидации ---
    private static void ValidatePassword(string password)
    {
        if (password.Length < 6)
            throw new DomainValidationException("The password is too short, it must be at least 6 characters long");
        if (!ContainsOnlyLatinLettersAndDigits(password))
            throw new DomainValidationException("The password contains invalid characters");
    }

    private static void ValidateLogin(string login)
    {
        if (login.Length < 4)
            throw new DomainValidationException("The login is too short, it must be at least 4 characters long");
        if (!ContainsOnlyLatinLettersAndDigits(login))
            throw new DomainValidationException("The login contains invalid characters");
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrEmpty(name))
            throw new DomainValidationException("The name is empty");
        if (!ContainsOnlyLatinAndRussianLetters(name)) // Используем новый метод
            throw new DomainValidationException("The name contains invalid characters. Only Latin and Russian letters are allowed.");
    }

    private static bool ContainsOnlyLatinLettersAndDigits(string str)
    {
        if (string.IsNullOrEmpty(str))
            return false;
        return Regex.IsMatch(str, @"^[a-zA-Z0-9]+$");
    }
    private static bool ContainsOnlyLatinAndRussianLetters(string str)
    {
        if (string.IsNullOrEmpty(str))
            return false;
        // Эта регулярка проверяет, что строка состоит только из латинских и русских букв
        return Regex.IsMatch(str, @"^[a-zA-Zа-яА-Я]+$");
    }

    // --- Методы для проверок ---
    public bool VerifyPassword(string password)
    {
        if (!BCrypt.Verify(password, Password))
            return false;
        return true;
    }

    private static string HashPassword(string password)
    {
        return BCrypt.HashPassword(password);
    }
}