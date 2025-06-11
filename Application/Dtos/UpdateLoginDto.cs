using System.ComponentModel.DataAnnotations;

namespace Application.Dtos;

public  record class UpdateLoginDto
{
    [Required] public string NewLogin { get; set; }
}