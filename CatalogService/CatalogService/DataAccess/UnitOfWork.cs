using CatalogService.DataAccess.Repositories;
using CatalogService.Models;

namespace CatalogService.DataAccess;

public class UnitOfWork : IDisposable
{
    private bool _isDisposed = false;
    private readonly AppDbContext _context;
    private Repository<Product> _products = default!;
    private Repository<Category> _categories = default!;
    private Repository<ProductCategory> _productCategories = default!;
    private Repository<Purchase> _purchases = default!;
    public Repository<Product> Products => _products ??= new Repository<Product>(_context);
    public Repository<Category> Categories => _categories ??= new Repository<Category>(_context);
    public Repository<ProductCategory> ProductCategories => _productCategories ??= new Repository<ProductCategory>(_context);
    public Repository<Purchase> Purchases => _purchases ??= new Repository<Purchase>(_context);

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public void Save()
    {
        _context.SaveChanges();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed && disposing)
        {
            _context.Dispose();
        }

        _isDisposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
