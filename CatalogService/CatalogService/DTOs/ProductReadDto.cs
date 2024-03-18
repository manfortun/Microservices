namespace CatalogService.DTOs;

public class ProductReadDto
{
    public int Id { get; set; }

    public string Name { get; set; } = default!;

    public double Price { get; set; }

    public string Description { get; set; } = default!;

    public string? ImageSource { get; set; }

    public virtual List<ProductCategoryReadDto> Category { get; set; } = default!;
}
