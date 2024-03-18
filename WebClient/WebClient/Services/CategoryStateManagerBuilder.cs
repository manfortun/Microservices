using WebClient.DTOs;

namespace WebClient.Services;

public class CategoryStateManagerBuilder : CategoryStateManager
{
    public CategoryStateManagerBuilder SetItems(IEnumerable<CategoryDto> categories)
    {
        Categories = categories;
        return this;
    }

    public CategoryStateManagerBuilder SetSelectedItems(IEnumerable<int> categoryIds)
    {
        foreach (var id in categoryIds)
        {
            _selectedCategories.Add(id);
        }

        return this;
    }

    public CategoryStateManager Build()
    {
        ArgumentNullException.ThrowIfNull(Categories);
        return this;
    }
}
