namespace WebClient.DTOs;

public class ReadPurchaseDto
{
    public string OwnerId { get; set; } = default!;

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public virtual ProductOnPurchaseDto Product { get; set; } = default!;
}
