using WebClient.DTOs;
using WebClient.Services.Interfaces;

namespace WebClient.Services;

public class PagingService : APagingService<ProductDto>
{
    public PagingService(int pageSize) : base(pageSize) { }

    public override IEnumerable<ProductDto> Get()
    {
        return Items
            .Skip((_activePage - 1) * _pageSize)
            .Take(_pageSize);
    }

    public override void SetItems(IEnumerable<ProductDto> items)
    {
        Items = items;
        _noOfPages = (int)Math.Ceiling((double)items.Count() / _pageSize);
        ActivePage = _activePage;
    }
}
