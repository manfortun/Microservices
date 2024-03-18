using System.ComponentModel.DataAnnotations;

namespace WebClient.DTOs;

public class CreateCategoryDto
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }
}
