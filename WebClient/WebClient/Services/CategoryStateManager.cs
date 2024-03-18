using WebClient.DTOs;

namespace WebClient.Services;

public class CategoryStateManager
{
    protected HashSet<int> _selectedCategories = new HashSet<int>();

    public IEnumerable<CategoryDto> Categories { get; protected set; } = default!;

    public int[] SelectedCategoryIds => [.. _selectedCategories];

    protected CategoryStateManager() { }

    public void Toggle(int categoryId)
    {
        if (!_selectedCategories.Add(categoryId))
        {
            _selectedCategories.Remove(categoryId);
        }
    }

    public bool HasCategory(int categoryId)
    {
        return _selectedCategories.Contains(categoryId);
    }

    public IEnumerable<CategoryDto> GetCategories()
    {
        return Categories;
    }

    public bool Any()
    {
        return _selectedCategories.Any();
    }

    public void Clear()
    {
        Categories = new List<CategoryDto>();
        _selectedCategories.Clear();
    }


    public IEnumerable<ProductCategoryDto> ToProductCategoryDtos(int productId)
    {
        return _selectedCategories.Select(c => new ProductCategoryDto
        {
            ProductId = productId,
            CategoryId = c
        });
    }
}
