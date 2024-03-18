using WebClient.DTOs;

namespace WebClient.Services;

public class BasketService
{
    private List<ReadPurchaseDto> _purchases = default!;
    public bool OnEditMode { get; set; } = false;

    /// <summary>
    /// Creates a new basket from a list of <paramref name="purchases"/>
    /// </summary>
    /// <param name="purchases"></param>
    public virtual void AddNewBasket(IEnumerable<ReadPurchaseDto> purchases)
    {
        _purchases = [.. purchases];
    }

    /// <summary>
    /// Gets the current basket
    /// </summary>
    /// <returns></returns>
    public virtual List<ReadPurchaseDto> GetBasket()
    {
        return _purchases;
    }

    /// <summary>
    /// Sets the count of the purchase object based from <paramref name="productId"/> and <paramref name="ownerId"/>
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="ownerId"></param>
    /// <param name="count"></param>
    public virtual void ChangePurchaseCount(int productId, string ownerId, int count)
    {
        ReadPurchaseDto? purchase = _purchases.Find(c => c.ProductId == productId && c.OwnerId == ownerId);
        if (purchase is ReadPurchaseDto purchaseDto)
        {
            purchaseDto.Quantity = count;

            if (purchaseDto.Quantity <= 0)
            {
                _purchases.Remove(purchaseDto);
            }
        }
    }

    /// <summary>
    /// Sums the current basket total
    /// </summary>
    /// <returns></returns>
    public virtual double GetBasketTotal()
    {
        return _purchases.Sum(p => p.Quantity * p.Product.Price);
    }

    /// <summary>
    /// Gets the number of items in the current basket
    /// </summary>
    /// <returns></returns>
    public virtual int GetNoOfItems()
    {
        return _purchases.Sum(c => c.Quantity);
    }

    /// <summary>
    /// Sums the total of a product with <paramref name="productId"/>
    /// </summary>
    /// <param name="productId"></param>
    /// <returns></returns>
    public virtual double GetProductTotal(int productId)
    {
        ReadPurchaseDto? purchase = _purchases?.Find(p => p.ProductId == productId) ?? default!;

        return purchase is not null ? purchase.Product.Price * purchase.Quantity : 0.00;
    }

    /// <summary>
    /// Removes all items in the basket
    /// </summary>
    public virtual void Clear()
    {
        _purchases.Clear();
    }
}
