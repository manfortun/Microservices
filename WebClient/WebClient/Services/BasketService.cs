using WebClient.DTOs;

namespace WebClient.Services;

public class BasketService
{
    private List<ReadPurchaseDto> _purchases = default!;
    public bool OnEditMode { get; set; } = false;

    public virtual void AddNewBasket(IEnumerable<ReadPurchaseDto> purchases)
    {
        _purchases = [.. purchases];
    }

    public virtual List<ReadPurchaseDto> GetBasket()
    {
        return _purchases;
    }

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

    public virtual double GetBasketTotal()
    {
        return _purchases.Sum(p => p.Quantity * p.Product.Price);
    }

    public virtual int GetNoOfItems()
    {
        return _purchases.Sum(c => c.Quantity);
    }

    public virtual double GetProductTotal(int productId)
    {
        ReadPurchaseDto? purchase = _purchases?.Find(p => p.ProductId == productId) ?? default!;

        return purchase is not null ? purchase.Product.Price * purchase.Quantity : 0.00;
    }

    public virtual void Clear()
    {
        _purchases.Clear();
    }
}
