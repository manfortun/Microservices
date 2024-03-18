﻿namespace CatalogService.Models;

public class ProductCategory
{
    public int ProductId { get; set; }
    public virtual Product Product { get; set; } = default!;

    public int CategoryId { get; set; }
    public virtual Category Category { get; set; } = default!;
}
