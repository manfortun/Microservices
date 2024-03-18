using Microsoft.AspNetCore.Mvc;
using WebClient.DTOs;
using WebClient.Services;
using WebClient.Services.HttpClients;
using WebClient.Services.Interfaces;

namespace WebClient.Controllers;

public class BasketController : Controller
{
    private readonly IHttpServiceWrapper _catalogService;
    private static BasketService _localBasket;
    public BasketController(IConfiguration config, AuthService localAuthService)
    {
        _catalogService = new HttpService<HttpCatalogService>(config, localAuthService);
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetCarts()
    {
        await SyncDbBasketToLocalBasket();

        return _localBasket.GetNoOfItems() > 0 ?
            PartialView("BasketSummaryPartialView", _localBasket) :
            PartialView("NoContentPartialView");
    }

    [HttpGet]
    public async Task<IActionResult> ToggleEditMode(bool onEditMode, bool save)
    {
        _localBasket.OnEditMode = onEditMode;

        if (save)
        {
            // sync the local basket to the database basket
            if (!onEditMode && !await MergeDbBasketWithLocalBasket())
            {
                TempData["error"] = "Unable to apply changes to your basket. Try again later.";
                return BadRequest();
            }
        }
        else
        {
            // resyncs the database basket to the local basket
            return RedirectToAction("getcarts");
        }

        return _localBasket.GetNoOfItems() > 0 ?
            PartialView("BasketSummaryPartialView", _localBasket) :
            PartialView("NoContentPartialView");
    }

    [HttpGet]
    public async Task<IActionResult> Checkout()
    {
        _localBasket.Clear();

        if (!await MergeDbBasketWithLocalBasket())
        {
            return BadRequest();
        }
        else
        {
            return PartialView("NoContentPartialView");
        }
    }

    [HttpGet]
    public IActionResult ChangeCount(int productId, string ownerId, int count)
    {
        _localBasket.ChangePurchaseCount(productId, ownerId, count);

        return PartialView("BasketSummaryPartialView", _localBasket);
    }

    /// <summary>
    /// Merge the local basket to database basket
    /// </summary>
    /// <returns></returns>
    private async Task<bool> MergeDbBasketWithLocalBasket()
    {
        HttpResponseMessage? response = await _catalogService.DeleteAsync(HttpContext, "DeleteBasket");
        var content = await HttpContentDeserializer.Deserialize<string>(response);

        if (content.Successful)
        {
            return await AddLocalBasketToDb();
        }

        return false;
    }

    /// <summary>
    /// Sets the local basket to database
    /// </summary>
    /// <returns></returns>
    private async Task<bool> AddLocalBasketToDb()
    {
        HttpResponseMessage? response = await _catalogService.PostAsync(HttpContext, _localBasket.GetBasket(), "SaveBasket");
        var content = await HttpContentDeserializer.Deserialize<string>(response);

        return content.Successful;
    }

    /// <summary>
    /// Gets the database basket and syncs into the local basket
    /// </summary>
    /// <returns></returns>
    private async Task<bool> SyncDbBasketToLocalBasket()
    {
        _localBasket = new BasketService();

        HttpResponseMessage? response = await _catalogService.GetAsync(HttpContext, "GetBasket");
        var content = await HttpContentDeserializer.Deserialize<IEnumerable<ReadPurchaseDto>>(response);

        if (content.Successful)
        {
            _localBasket.AddNewBasket(content.Content);
        }

        return content.Successful;
    }
}
