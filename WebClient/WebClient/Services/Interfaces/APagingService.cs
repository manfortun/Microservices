namespace WebClient.Services.Interfaces;

public abstract class APagingService<T>
{
    protected readonly int _pageSize;
    protected int _activePage = 1;
    protected int _noOfPages = 0;
    public IEnumerable<T> Items { get; protected set; }

    protected APagingService(int pageSize)
    {
        _pageSize = pageSize;
        Items = new List<T>();
    }

    /// <summary>
    /// Current page to display
    /// </summary>
    public int ActivePage
    {
        get => _activePage;
        set
        {
            if (value < 1)
            {
                _activePage = _noOfPages;
            }
            else if (value > _noOfPages)
            {
                _activePage = 1;
            }
            else
            {
                _activePage = value;
            }
        }
    }

    /// <summary>
    /// No of pages available based from the <see cref="Activepage"/> and <see cref="PageSize"/>.
    /// </summary>
    public int NoOfPages => _noOfPages;

    /// <summary>
    /// No of items per page
    /// </summary>
    public int PageSize => _pageSize;

    /// <summary>
    /// Sets items of type <typeparamref name="T"/> to the whole book
    /// </summary>
    /// <param name="items"></param>
    public abstract void SetItems(IEnumerable<T> items);

    /// <summary>
    /// Get the items of the current page
    /// </summary>
    /// <returns></returns>
    public abstract IEnumerable<T> Get();
}
