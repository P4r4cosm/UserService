using System.ComponentModel.DataAnnotations;

namespace Application.Dtos;

public record class UpdatePasswordDto
{
    [Required]
    public string NewPassword { get; set; }
}