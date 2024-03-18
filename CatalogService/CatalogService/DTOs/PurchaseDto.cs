using System.ComponentModel.DataAnnotations;

namespace CatalogService.DTOs;

public class PurchaseDto
{
    [Required]
    public string OwnerId { get; set; } = default!;
    [Required]
    public int ProductId { get; set; }
    public virtual ProductReadDto? Product { get; set; } = default!;
    [Required]
    public int Quantity { get; set; }
}
