namespace WebClient.DTOs;

public class ProductOnPurchaseDto
{
    public int Id { get; set; }

    public string Name { get; set; } = default!;

    public double Price { get; set; }
}
