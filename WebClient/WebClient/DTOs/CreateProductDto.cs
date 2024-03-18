using System.ComponentModel.DataAnnotations;

namespace WebClient.DTOs;

public class CreateProductDto
{
    [Required]
    public string Name { get; set; } = default!;

    [Required]
    public double Price { get; set; }

    [MaxLength(150)]
    public string? Description { get; set; }

    public string? ImageSource { get; set; }
}
