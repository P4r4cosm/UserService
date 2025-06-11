using Domain.Entities;

namespace Application.Dtos;

public class UpdateUserInfoDto
{
    public string Name { get; set; }
    public Gender Gender { get; set; }
    public DateTime? Birthday { get; set; }
}