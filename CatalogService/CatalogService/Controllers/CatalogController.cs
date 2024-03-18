using AutoMapper;
using CatalogService.DataAccess;
using CatalogService.DTOs;
using CatalogService.Models;
using CatalogService.SyncDataServices.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CatalogService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CatalogController : ControllerBase
{
    private readonly UnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly ICommandDataClient _commandDataClient;

    public CatalogController(AppDbContext context, IMapper mapper, IHttpContextAccessor contextAccessor, ICommandDataClient commandDataClient)
    {
        _unitOfWork = new UnitOfWork(context);
        _mapper = mapper;
        _contextAccessor = contextAccessor;
        _commandDataClient = commandDataClient;
    }

    [HttpGet]
    public IActionResult TestConnection()
    {
        return Ok();
    }

    [HttpGet(nameof(GetProducts))]
    public ActionResult<IEnumerable<ProductReadDto>> GetProducts()
    {
        IEnumerable<Product> products = _unitOfWork.Products.Get(includeProperties: "Category");

        var productDtos = _mapper.Map<IEnumerable<ProductReadDto>>(products);
        
        return Ok(productDtos);
    }

    [HttpGet(nameof(GetProductById))]
    public ActionResult<ProductReadDto> GetProductById(int id)
    {
        IEnumerable<Product> products = _unitOfWork.Products.Get(includeProperties: "Category");

        Product? product = products.FirstOrDefault(p => p.Id == id);

        if (product is null)
        {
            return NotFound();
        }

        var productDto = _mapper.Map<ProductReadDto>(product);
        return Ok(productDto);
    }

    [HttpPost(nameof(CreateProduct))]
    public ActionResult<ProductReadDto> CreateProduct(Product product)
    {
        EntityEntry<Product> entityEntry = _unitOfWork.Products.Insert(product);
        _unitOfWork.Save();

        if (!entityEntry.IsKeySet)
        {
            return BadRequest();
        }

        ProductReadDto productRead = _mapper.Map<ProductReadDto>(entityEntry.Entity);
        return Ok(productRead);
    }

    [HttpPost(nameof(AddCategory))]
    public ActionResult<IEnumerable<CategoryReadDto>> AddCategory(Category category)
    {
        if (!IsUnique(category))
        {
            return BadRequest("A category with this name already exists.");
        }

        EntityEntry<Category> entityEntry = _unitOfWork.Categories.Insert(category);

        if (entityEntry.State == EntityState.Added)
        {
            _unitOfWork.Save();
            return Ok("Category saved.");
        }

        return BadRequest("Can't add the category.");
    }

    [HttpGet(nameof(GetCategory))]
    public ActionResult<CategoryReadDto> GetCategory(int id)
    {
        Category? category = _unitOfWork.Categories.FindByKeys(id);

        if (category is null)
        {
            return NotFound();
        }

        return Ok(category);
    }

    [HttpPost(nameof(EditCategory))]
    public ActionResult<string> EditCategory(Category category)
    {
        if (!IsUnique(category))
        {
            return BadRequest("A category with this name already exists.");
        }

        EntityEntry<Category> entityEntry = _unitOfWork.Categories.Update(category);
        if (entityEntry.State == EntityState.Modified)
        {
            _unitOfWork.Save();
            return Ok("Category saved;");
        }

        return BadRequest("Unable to save category");
    }

    [HttpGet(nameof(GetCategories))]
    public ActionResult<IEnumerable<CategoryReadDto>> GetCategories(int? productId)
    {
        IEnumerable<Category> categories;

        if (productId is not null)
        {
            categories = _unitOfWork.ProductCategories
                .Get(pc => pc.ProductId == productId, "Category")
                .Select(pc => pc.Category);
        }
        else
        {
            categories = _unitOfWork.Categories.Get();
        }

        var categoryDtos = _mapper.Map<IEnumerable<CategoryReadDto>>(categories);
        return Ok(categoryDtos);
    }

    [HttpDelete(nameof(DeleteCategory))]
    public ActionResult<string> DeleteCategory(int id)
    {
        EntityEntry<Category> entityEntry = _unitOfWork.Categories.Delete(id);
        if (entityEntry is not null && entityEntry.State == EntityState.Deleted)
        {
            _unitOfWork.Save();
            return Ok("Category has been deleted successfully.");
        }

        return NotFound();
    }

    [HttpPost(nameof(SaveProduct))]
    public ActionResult<ProductReadDto> SaveProduct(Product product)
    {
        EntityEntry<Product> savedProduct = _unitOfWork.Products.Update(product);

        if (savedProduct.State == EntityState.Modified)
        {
            _unitOfWork.Save();

            return Ok(savedProduct.Entity);
        }
        else
        {
            return BadRequest();
        }
    }

    [HttpPost(nameof(SaveProductCategories))]
    public ActionResult<string> SaveProductCategories(IEnumerable<ProductCategoryReadDto> categories)
    {
        try
        {
            int[] ids = categories.Select(c => c.ProductId).Distinct().ToArray();

            if (ids.Any())
            {
                IEnumerable<ProductCategory> toDelete = _unitOfWork.ProductCategories.Get(pc => ids.Contains(pc.ProductId));
        
                foreach (var delete in toDelete)
                {
                    _ = _unitOfWork.ProductCategories.Delete(delete);
                }
            }

            foreach (var add in categories)
            {
                _unitOfWork.ProductCategories.Insert(_mapper.Map<ProductCategory>(add));
            }

            _unitOfWork.Save();
            return Ok("Categories saved");
        }
        catch
        {
            return BadRequest();
        }
    }

    [HttpDelete(nameof(DeleteProduct))]
    public ActionResult<string> DeleteProduct(int id)
    {
        EntityEntry<Product> entityEntry = _unitOfWork.Products.Delete(id);
        if (entityEntry is not null && entityEntry.State == EntityState.Deleted)
        {
            _unitOfWork.Save();
            return Ok("Product has been deleted successfully.");
        }

        return NotFound();
    }

    [HttpGet(nameof(GetBasket))]
    public async Task<ActionResult<IEnumerable<PurchaseDto>>> GetBasket()
    {
        string? ownerId = await GetUserIdFromHeader();
        IEnumerable<Purchase> purchases = _unitOfWork.Purchases.Get(p => p.OwnerId == ownerId);

        var purchasesDto = _mapper.Map<IEnumerable<PurchaseDto>>(purchases);
        return Ok(purchasesDto);
    }

    [HttpGet(nameof(GetPurchase))]
    public async Task<ActionResult<PurchaseDto>> GetPurchase(int productId)
    {
        string? ownerId = await GetUserIdFromHeader();
        Purchase? purchase = _unitOfWork.Purchases.FindByKeys(ownerId, productId);

        return Ok(_mapper.Map<PurchaseDto>(purchase));
    }

    [HttpPost(nameof(AddPurchase))]
    public async Task<ActionResult<Purchase>> AddPurchase(PurchaseDto purchase)
    {
        string? ownerId = await GetUserIdFromHeader();

        purchase.OwnerId = ownerId;

        EntityEntry<Purchase> entityEntry = _unitOfWork.Purchases.Insert(_mapper.Map<Purchase>(purchase));

        if (entityEntry.State == EntityState.Added)
        {
            _unitOfWork.Save();
            return Ok(entityEntry.Entity);
        }

        return BadRequest("Unable to save purchase.");
    }

    [HttpPost(nameof(SavePurchase))]
    public ActionResult<Purchase> SavePurchase(PurchaseDto purchase)
    {
        EntityEntry<Purchase> entityEntry = _unitOfWork.Purchases.Update(_mapper.Map<Purchase>(purchase));

        if (entityEntry.State == EntityState.Modified)
        {
            _unitOfWork.Save();
            return Ok(entityEntry.Entity);
        }

        return BadRequest("Unable to save purchase.");
    }

    [HttpPost(nameof(SaveBasket))]
    public ActionResult<string> SaveBasket(IEnumerable<Purchase> purchases)
    {
        foreach (var purchase in purchases)
        {
            purchase.Product = null;
            _ = _unitOfWork.Purchases.Insert(purchase);
        }

        _unitOfWork.Save();

        return Ok("Basket saved.");
    }

    [HttpDelete(nameof(DeleteBasket))]
    public async Task<ActionResult<string>> DeleteBasket()
    {
        string ownerId = await GetUserIdFromHeader();
        IEnumerable<Purchase> purchases = _unitOfWork.Purchases.Get(p => p.OwnerId == ownerId);

        foreach (var purchase in purchases)
        {
            _ = _unitOfWork.Purchases.Delete(purchase);
        }

        _unitOfWork.Save();

        return Ok("Basket deleted.");
    }

    private bool IsUnique(Category category)
    {
        string[] names = _unitOfWork.Categories
            .Get(c => c.Id != category.Id)
            .Select(c => c.Name)
            .ToArray();

        string[] normalizedNames = names.Select(n => n.Replace(" ", "").ToUpper()).ToArray();

        return !normalizedNames.Contains(category.Name);
    }

    private async Task<string> GetUserIdFromHeader()
    {
        var request = _contextAccessor.HttpContext?.Request;

        if (request is null ||
            !request.Headers.TryGetValue("Authorization", out var authorizationToken))
        {
            return default!;
        }

        return await _commandDataClient.GetId(authorizationToken.ToString());
    }
}
