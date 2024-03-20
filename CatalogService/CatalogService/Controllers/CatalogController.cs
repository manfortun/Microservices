using AutoMapper;
using CatalogService.DataAccess;
using CatalogService.DTOs;
using CatalogService.Models;
using CatalogService.Services;
using CatalogService.SyncDataServices.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CatalogService.Controllers;

[AuthHeaderFilter]
[ApiController]
[Route("api/[controller]")]
public class CatalogController : ControllerBase
{
    private readonly UnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly ICommandDataClient _commandDataClient;

    public CatalogController(AppDbContext context,
        IMapper mapper,
        IHttpContextAccessor contextAccessor,
        ICommandDataClient commandDataClient)
    {
        _unitOfWork = new UnitOfWork(context);
        _mapper = mapper;
        _contextAccessor = contextAccessor;
        _commandDataClient = commandDataClient;
    }

    /// <summary>
    /// For testing connection
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public IActionResult TestConnection()
    {
        return Ok();
    }

    /// <summary>
    /// Obtain all products from the database
    /// </summary>
    /// <returns></returns>
    [HttpGet(nameof(GetProducts))]
    public ActionResult<IEnumerable<ProductReadDto>> GetProducts()
    {
        IEnumerable<Product> products = _unitOfWork.Products.Get(includeProperties: "Category");

        var productDtos = _mapper.Map<IEnumerable<ProductReadDto>>(products);

        return Ok(productDtos);
    }

    /// <summary>
    /// Get a product of specific <paramref name="id"/>
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Insert a <paramref name="product"/> into the database
    /// </summary>
    /// <param name="product"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Insert a <paramref name="category"/> into the database
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Get a category of specific <paramref name="id"/>
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Edits the <paramref name="category"/>
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Retrieves the categories of a product of specific <paramref name="productId"/>
    /// </summary>
    /// <param name="productId"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Removes a category of <paramref name="id"/> from the database
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Updates a <paramref name="product"/> in the database
    /// </summary>
    /// <param name="product"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Saves an enumerable of <paramref name="categories"/> into the database
    /// </summary>
    /// <param name="categories"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Removes a product of specific <paramref name="id"/>
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Obtains the basket of current user. The JWT token must be included in the request header.
    /// </summary>
    /// <returns></returns>
    [HttpGet(nameof(GetBasket))]
    public async Task<ActionResult<IEnumerable<PurchaseDto>>> GetBasket()
    {
        string? ownerId = await GetUserIdFromHeader();

        if (string.IsNullOrEmpty(ownerId))
        {
            return Unauthorized();
        }

        IEnumerable<Purchase> purchases = _unitOfWork.Purchases.Get(p => p.OwnerId == ownerId);

        var purchasesDto = _mapper.Map<IEnumerable<PurchaseDto>>(purchases);
        return Ok(purchasesDto);
    }

    /// <summary>
    /// Obtains the purchase object of current user whose product id = <paramref name="productId"/>.
    /// The JWT token must be included in the request header.
    /// </summary>
    /// <param name="productId"></param>
    /// <returns></returns>
    [HttpGet(nameof(GetPurchase))]
    public async Task<ActionResult<PurchaseDto>> GetPurchase(int productId)
    {
        string? ownerId = await GetUserIdFromHeader();

        if (string.IsNullOrEmpty(ownerId))
        {
            return Unauthorized();
        }

        Purchase? purchase = _unitOfWork.Purchases.FindByKeys(ownerId, productId);

        return Ok(_mapper.Map<PurchaseDto>(purchase));
    }

    /// <summary>
    /// Adds a purchase object to the current user.
    /// The JWT token must be included in the request header.
    /// </summary>
    /// <param name="purchase"></param>
    /// <returns></returns>
    [HttpPost(nameof(AddPurchase))]
    public async Task<ActionResult<Purchase>> AddPurchase(CreatePurchaseDto purchase)
    {
        string? ownerId = await GetUserIdFromHeader();

        if (string.IsNullOrEmpty(ownerId))
        {
            return Unauthorized();
        }

        purchase.OwnerId = ownerId;

        EntityEntry<Purchase> entityEntry = _unitOfWork.Purchases.Insert(_mapper.Map<Purchase>(purchase));

        if (entityEntry.State == EntityState.Added)
        {
            _unitOfWork.Save();
            return Ok(entityEntry.Entity);
        }

        return BadRequest("Unable to save purchase.");
    }

    /// <summary>
    /// Updates a purchase object of a user.
    /// The JWT token must be included in the request header.
    /// </summary>
    /// <param name="purchase"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Inserts an enumerable of <paramref name="purchases"/> into the database.
    /// </summary>
    /// <param name="purchases"></param>
    /// <returns></returns>
    [HttpPost(nameof(SaveBasket))]
    public ActionResult<string> SaveBasket(IEnumerable<Purchase> purchases)
    {
        foreach (var purchase in purchases)
        {
            purchase.Product = default!;
            _ = _unitOfWork.Purchases.Insert(purchase);
        }

        _unitOfWork.Save();

        return Ok("Basket saved.");
    }

    /// <summary>
    /// Removes a basket of a user.
    /// The JWT token must be included in the request header.
    /// </summary>
    /// <returns></returns>
    [HttpDelete(nameof(DeleteBasket))]
    public async Task<ActionResult<string>> DeleteBasket()
    {
        string ownerId = await GetUserIdFromHeader();

        if (string.IsNullOrEmpty(ownerId))
        {
            return Unauthorized();
        }

        IEnumerable<Purchase> purchases = _unitOfWork.Purchases.Get(p => p.OwnerId == ownerId);

        foreach (var purchase in purchases)
        {
            _ = _unitOfWork.Purchases.Delete(purchase);
        }

        _unitOfWork.Save();

        return Ok("Basket deleted.");
    }

    /// <summary>
    /// Determines if a <paramref name="category"/> name is unique.
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    private bool IsUnique(Category category)
    {
        string[] names = _unitOfWork.Categories
            .Get(c => c.Id != category.Id)
            .Select(c => c.Name)
            .ToArray();

        string[] normalizedNames = names.Select(n => n.Replace(" ", "").ToUpper()).ToArray();

        return !normalizedNames.Contains(category.Name);
    }

    /// <summary>
    /// Retrieves the user ID from the request header.
    /// </summary>
    /// <returns></returns>
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
