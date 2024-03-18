using WebClient.DTOs;

namespace WebClient.Services;

public class PagingWithSearchService : PagingService
{
    public PagingWithSearchService(PagingService service, string searchString) : base(service.PageSize)
    {
        if (string.IsNullOrEmpty(searchString))
        {
            SetItems(service.Items);
        }
        else
        {
            SetItems(service.Items.Where(p => SearchFunction(p, searchString)));
        }
    }

    private bool SearchFunction(ProductDto product, string searchKey)
    {
        if (product.Name.Contains(searchKey, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        string[] categoryNames = product.Category?
            .Select(c => c.Category?.Name)
            .OfType<string>()
            .ToArray() ?? [];

        if (categoryNames.Any(c => c.Contains(searchKey, StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        return false;
    }
}
