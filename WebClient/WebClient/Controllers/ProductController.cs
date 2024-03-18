using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using WebClient.DTOs;
using WebClient.Models;
using WebClient.Services;
using WebClient.Services.HttpClients;
using WebClient.Services.Interfaces;

namespace WebClient.Controllers;

public class ProductController : Controller
{
    private static CategoryStateManager _categoryStateManager;
    private readonly IHttpService _catalogService;
    private readonly IHttpService _authService
        ;
    private readonly AuthService _localAuthService;

    public ProductController(IConfiguration config, AuthService localAuthService)
    {
        _catalogService = new HttpService<HttpCatalogService>(config, localAuthService);
        _authService = new HttpService<HttpAuthService>(config, localAuthService);
        _localAuthService = localAuthService;
    }

    [HttpPost]
    public async Task<IActionResult> ViewProduct(int id)
    {
        HttpResponseMessage? response = await _catalogService.AddParameter("id", id).GetAsync(HttpContext, "GetProductById");
        var contentDeserializer = await HttpContentDeserializer.Deserialize<ProductDto>(response);

        if (contentDeserializer.Successful)
        {
            return View(contentDeserializer.Content);
        }

        return View("Error", new ErrorViewModel { Message = "Oops! Something went wrong. We're trying to fix it." });
    }

    [HttpGet]
    public IActionResult CreateProduct()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct(CreateProductDto createProductDto)
    {
        var productSaveResult = await SaveObjectToCatalogService<ProductDto>(createProductDto, "CreateProduct");

        if (productSaveResult.Successful)
        {
            if (_categoryStateManager.Any())
            {
                ProductDto product = productSaveResult.Content;
                IEnumerable<ProductCategoryDto> newCategories = _categoryStateManager.ToProductCategoryDtos(product.Id);

                _ = await SaveObjectToCatalogService<ProductDto>(newCategories, "SaveProductCategories");
                _categoryStateManager.Clear();
            }
        }
        else
        {
            TempData["error"] = "We can't save this product right now. Try again later";
            return View();
        }

        TempData["success"] = "Product added successfully";
        return RedirectToAction("Index", "Home");
    }

    [HttpGet, HttpPost]
    public async Task<IActionResult> GetCategories(int id)
    {
        HttpResponseMessage? response = await _catalogService.GetAsync(HttpContext, "GetCategories");
        var result = await HttpContentDeserializer.Deserialize<IEnumerable<CategoryDto>>(response);

        if (result.Successful)
        {
            var builder = new CategoryStateManagerBuilder().SetItems(result.Content);

            if (id > 0)
            {
                var productCategories = await GetProductCategories(id);

                builder.SetSelectedItems(productCategories.Select(pc => pc.Id));
            }

            _categoryStateManager = builder.Build();
            return PartialView("CategoryTogglePartialView", _categoryStateManager);
        }

        return View("Error", new ErrorViewModel { Message = "Unable to get categories. Try again later." });
    }

    [HttpPost]
    public async Task<IActionResult> AddToBasket(int id)
    {
        HttpResponseMessage? getProductResponse = await _catalogService.AddParameter("id", id).GetAsync(HttpContext, "GetProductById");
        var getProductContent = await HttpContentDeserializer.Deserialize<ProductDto>(getProductResponse);

        if (!getProductContent.Successful)
        {
            return NotFound();
        }

        ProductDto product = getProductContent.Content;
        HttpResponseMessage? getPurchaseResponse = await _catalogService
            .AddParameter("productId", product.Id)
            .GetAsync(HttpContext, "GetPurchase");
        var getPurchaseContent = await HttpContentDeserializer.Deserialize<PurchaseDto>(getPurchaseResponse);

        PurchaseDto purchase = getPurchaseContent.Content;

        if (purchase is null)
        {
            var createPurchase = new PurchaseDto
            {
                ProductId = product.Id,
                Quantity = 1
            };

            HttpResponseMessage? addResponse = await _catalogService.PostAsync(HttpContext, createPurchase, "AddPurchase");
        }
        else
        {
            purchase.Quantity++;
            await _catalogService.PostAsync(HttpContext, purchase, "SavePurchase");
        }

        return Ok(product.Name);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        HttpResponseMessage? response = await _catalogService.AddParameter("id", id).GetAsync(HttpContext, "GetProductById");
        var result = await HttpContentDeserializer.Deserialize<ProductDto>(response);

        if (!result.Connected)
        {
            return View("Error", new ErrorViewModel { Message = "Something went wrong. Try again later." });
        }

        if (!result.Successful)
        {
            return View("Error", new ErrorViewModel { Message = "This product is no longer available." });
        }

        return View(result.Content);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(ProductDto product)
    {
        HttpResponseMessage? response = await _catalogService.PostAsync(HttpContext, product, "SaveProduct");
        var result = await HttpContentDeserializer.Deserialize<ProductDto>(response);

        if (result.Successful)
        {
            response = await _catalogService.PostAsync(HttpContext, _categoryStateManager.ToProductCategoryDtos(product.Id), "SaveProductCategories");
            var catResult = await HttpContentDeserializer.Deserialize<string>(response);

            if (catResult.Successful)
            {
                TempData["success"] = "Product successfully updated";
                return RedirectToAction("index", "home");
            }
        }

        return View("Error", new ErrorViewModel { Message = "Something went wrong. Try again later." });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        HttpResponseMessage? response = await _catalogService.AddParameter("id", id).DeleteAsync(HttpContext, "DeleteProduct");
        var content = await HttpContentDeserializer.Deserialize<string>(response);

        if (content.Successful)
        {
            TempData["success"] = "Product has been deleted.";
        }
        else
        {
            TempData["error"] = "Can't delete the product now. Try again later.";
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult ToggleCategory(int id)
    {
        _categoryStateManager.Toggle(id);

        return PartialView("CategoryTogglePartialView", _categoryStateManager);
    }


    private async Task<DeserializationResult<T>> SaveObjectToCatalogService<T>(object obj, params string[] endpoints)
        where T : class
    {
        HttpResponseMessage? response = await _catalogService.PostAsync(HttpContext, obj, endpoints);
        return await HttpContentDeserializer.Deserialize<T>(response);
    }

    private async Task<IEnumerable<CategoryDto>> GetProductCategories(int productId)
    {
        HttpResponseMessage? response = await _catalogService.AddParameter("productId", productId).GetAsync(HttpContext, "GetCategories");
        var result = await HttpContentDeserializer.Deserialize<IEnumerable<CategoryDto>>(response);

        return result.Content;
    }
}
