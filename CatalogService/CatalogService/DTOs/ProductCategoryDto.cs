namespace CatalogService.DTOs;

public class ProductCategoryDto
{
    public required int ProductId { get; set; }
    public ProductReadDto? Product { get; set; }
    public required int CategoryId { get; set; }
    public CategoryReadDto? Category { get; set; }
}
