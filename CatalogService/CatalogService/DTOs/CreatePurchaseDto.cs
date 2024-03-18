namespace CatalogService.DTOs;

public class CreatePurchaseDto
{
    public string OwnerId { get; set; } = default!;
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
