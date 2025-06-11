using Domain.Entities;

namespace Application.Dtos;

public record class UserDto
{
    public Guid Guid { get; set; }
    public string Login { get; set; }
    public string Name { get; set; }
    public Gender Gender { get; set; }
    public DateTime? Birthday { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedOn { get; set; }
}