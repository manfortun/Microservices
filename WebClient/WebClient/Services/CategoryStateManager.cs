using WebClient.DTOs;

namespace WebClient.Services;

public class CategoryStateManager
{
    protected HashSet<int> _selectedCategories = new HashSet<int>();

    public IEnumerable<CategoryDto> Categories { get; protected set; } = default!;

    public int[] SelectedCategoryIds => [.. _selectedCategories];

    protected CategoryStateManager() { }

    /// <summary>
    /// Adds or removes a <paramref name="categoryId"/> from selected categories.
    /// </summary>
    /// <param name="categoryId"></param>
    public void Toggle(int categoryId)
    {
        if (!_selectedCategories.Add(categoryId))
        {
            _selectedCategories.Remove(categoryId);
        }
    }

    /// <summary>
    /// Checks if <paramref name="categoryId"/> is selected
    /// </summary>
    /// <param name="categoryId"></param>
    /// <returns></returns>
    public bool HasCategory(int categoryId)
    {
        return _selectedCategories.Contains(categoryId);
    }

    /// <summary>
    /// Retrieves all categories
    /// </summary>
    /// <returns></returns>
    public IEnumerable<CategoryDto> GetCategories()
    {
        return Categories;
    }

    /// <summary>
    /// Checks if there are any selected categories
    /// </summary>
    /// <returns></returns>
    public bool Any()
    {
        return _selectedCategories.Any();
    }

    /// <summary>
    /// Clears the contents including the selected categories
    /// </summary>
    public void Clear()
    {
        Categories = new List<CategoryDto>();
        _selectedCategories.Clear();
    }

    /// <summary>
    /// Retrieves all selected categories
    /// </summary>
    /// <param name="productId"></param>
    /// <returns></returns>

    public IEnumerable<ProductCategoryDto> ToProductCategoryDtos(int productId)
    {
        return _selectedCategories.Select(c => new ProductCategoryDto
        {
            ProductId = productId,
            CategoryId = c
        });
    }
}
