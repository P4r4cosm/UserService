using System.Text.RegularExpressions;

namespace Domain.Entities;

public class User
{
    public Guid Guid { get; set; }
    public string Login { get; set; } //Уникальный Логин (запрещены все символы кроме латинских букв и цифр)
    public string Password { get; set; } //Пароль(запрещены все символы кроме латинских букв и цифр)
    public string Name { get; set; } //Имя (запрещены все символы кроме латинских и русских букв)
    public int Gender { get; set; } //0 - женщина, 1 - мужчина, 2 - неизвестно
    public DateTime? Birthday { get; set; }
    public bool Admin { get; set; } //Указание - является ли пользователь админом
    public DateTime CreatedAt { get; set; } //Дата создания пользователя
    public string CreatedBy { get; set; }// Пользователя, от имени которого этот пользователь создан
    public DateTime ModifiedOn { get; set; } //Дата изменения пользователя
    public string ModifiedBy { get; set; } //Логин Пользователя, от имени которого этот пользователь изменён
    public DateTime RevokedOn { get; set; } // Дата удаления пользователя
    public string RevokedBy { get; set; }//Логин Пользователя, от имени которого этот пользователь удалён
    
    private static string ValidatePassword(string password)
    {
        if (password.Length < 6)
            throw new ArgumentException("The password is too short, it must be at least 6 characters long");
        if (ContainsOnlyLatinLettersAndDigits(password))
            return password;
        throw new ArgumentException("The password contains invalid characters");
    }

    private static string ValidateLogin(string login)
    {
        if (login.Length<4)
            throw new ArgumentException("The login is too short, it must be at least 4 characters long");
        if (ContainsOnlyLatinLettersAndDigits(login))
            return login;
        throw new ArgumentException("The login contains invalid characters");
    }

    private static string ValidateName(string name)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("The name is empty");
        if (ContainsOnlyLatinLettersAndDigits(name))
            return name;
        throw new ArgumentException("The name contains invalid characters");
    }

    private static bool ContainsOnlyLatinLettersAndDigits(string str)
    {
        if (string.IsNullOrEmpty(str))
            return false;
        return Regex.IsMatch(str, @"^[a-zA-Z0-9]+$");
    }
}