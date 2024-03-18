using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CatalogService.Models;

public class Purchase
{
    [Required]
    public string OwnerId { get; set; } = default!;

    [Required]
    public int ProductId { get; set; }
    public virtual Product Product { get; set; } = default!;

    [DefaultValue(0)]
    public int Quantity { get; set; }
}
