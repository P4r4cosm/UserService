using Domain.Entities;

namespace Application.Dtos;

public record class CreateUserDto
{
    public string Login { get; set; }
    public string Password { get; set; }
    public string Name { get; set; }
    public Gender Gender { get; set; }
    public DateTime? Birthday { get; set; }
    public bool IsAdmin { get; set; }
}