using Microsoft.AspNetCore.Mvc;
using WebClient.DTOs;
using WebClient.Models;
using WebClient.Services;
using WebClient.Services.HttpClients;
using WebClient.Services.Interfaces;

namespace WebClient.Controllers;

public class HomeController : Controller
{
    private static readonly PagingService _pagingService = new PagingService(12);
    private readonly IHttpService _httpService;

    public HomeController(IConfiguration config, AuthService authService)
    {
        _httpService = new HttpService<HttpCatalogService>(config, authService);
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetItems(int? pageNo)
    {
        HttpResponseMessage? response = await _httpService.GetAsync(HttpContext, "GetProducts");

        try
        {
            var content = await HttpContentDeserializer.Deserialize<IEnumerable<ProductDto>>(response);

            if (!content.Connected)
            {
                return RedirectToAction("Error", new ErrorViewModel { Message = "Oops! Something went wrong. We're trying to fix it." });
            }

            if (!content.Successful)
            {
                return PartialView("Error", new ErrorViewModel { Message = "No items listed for now. Try again later." });
            }

            _pagingService.SetItems(content.Content);

            if (pageNo is int intPageNo)
            {
                _pagingService.ActivePage = intPageNo;
            }

            return PartialView("ProductsDisplayPartialView", _pagingService);
        }
        catch
        {
            return RedirectToAction("Error", new ErrorViewModel { Message = "Oops! Something went wrong. We're trying to fix it." });
        }
    }

    [HttpGet]
    public IActionResult Search(string searchString)
    {
        var searchWrapper = new PagingWithSearchService(_pagingService, searchString);

        return PartialView("ProductsDisplayPartialView", searchWrapper);
    }
}
