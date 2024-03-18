namespace WebClient.DTOs;

public class ProductCategoryDto
{
    public required int ProductId { get; set; }
    public ProductDto? Product { get; set; }
    public required int CategoryId { get; set; }
    public CategoryDto? Category { get; set; }
}
