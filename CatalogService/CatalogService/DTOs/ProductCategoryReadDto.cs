namespace CatalogService.DTOs;

public class ProductCategoryReadDto
{
    public required int ProductId { get; set; }
    public required int CategoryId { get; set; }
    public CategoryReadDto? Category { get; set; }
}
