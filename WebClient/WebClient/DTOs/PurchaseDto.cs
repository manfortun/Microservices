using System.ComponentModel.DataAnnotations;

namespace WebClient.DTOs;

public class PurchaseDto
{
    [Required]
    public string OwnerId { get; set; } = default!;
    [Required]
    public int ProductId { get; set; }
    [Required]
    public int Quantity { get; set; }
}
