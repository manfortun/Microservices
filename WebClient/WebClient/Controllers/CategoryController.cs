using Microsoft.AspNetCore.Mvc;
using WebClient.DTOs;
using WebClient.Models;
using WebClient.Services;
using WebClient.Services.HttpClients;
using WebClient.Services.Interfaces;

namespace WebClient.Controllers;

public class CategoryController : Controller
{
    private readonly IHttpService _httpService;
    public CategoryController(IConfiguration config, AuthService authService)
    {
        _httpService = new HttpService<HttpCatalogService>(config, authService);
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        HttpResponseMessage? response = await _httpService.GetAsync(HttpContext, "GetCategories");
        var content = await HttpContentDeserializer.Deserialize<IEnumerable<CategoryDto>>(response);

        if (!content.Connected)
        {
            return View("Error", new ErrorViewModel { Message = "Something went wrong. Try again later." });
        }
        if (content.Successful)
        {
            IEnumerable<CategoryDto> categories = content.Content;
            return View(categories);
        }

        return View("Error", new ErrorViewModel { Message = "Something went wrong. Try again later." });
    }

    [HttpGet]
    public IActionResult Add()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Add(CreateCategoryDto model)
    {
        HttpResponseMessage? response = await _httpService.PostAsync(HttpContext, model, "AddCategory");
        var content = await HttpContentDeserializer.Deserialize<string>(response);

        if (!content.Connected)
        {
            TempData["error"] = "Something went wrong. Try again later.";
            return View();
        }
        if (!content.Successful)
        {
            ModelState.AddModelError("", content.Content);
            return View();
        }

        TempData["success"] = "Category added.";
        return RedirectToAction("Index", "Category");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        HttpResponseMessage? response = await _httpService.AddParameter("id", id).GetAsync(HttpContext, "GetCategory");
        var content = await HttpContentDeserializer.Deserialize<CategoryDto>(response);

        if (!content.Connected)
        {
            TempData["error"] = "Something went wrong. Try again later.";
            return RedirectToAction("index", "category");
        }
        if (!content.Successful)
        {
            return View("Error", new ErrorViewModel { Message = "This category doesn't exist anymore." });
        }

        return View(content.Content);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(CategoryDto model)
    {
        HttpResponseMessage? response = await _httpService.PostAsync(HttpContext, model, "EditCategory");
        var content = await HttpContentDeserializer.Deserialize<string>(response);

        if (!content.Connected)
        {
            TempData["error"] = "Something went wrong. Try again later.";
            return View();
        }
        if (!content.Successful)
        {
            ModelState.AddModelError("", content.Content);
            return View();
        }

        TempData["success"] = "Product successfully changed.";
        return RedirectToAction("index", "category");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        HttpResponseMessage? response = await _httpService.AddParameter("id", id).DeleteAsync(HttpContext, "DeleteCategory");
        var content = await HttpContentDeserializer.Deserialize<string>(response);

        if (!content.Connected)
        {
            TempData["error"] = "Something went wrong. Try again later.";
            return RedirectToAction("index", "category");
        }

        if (!content.Successful)
        {
            ModelState.AddModelError("", content.Content);
            return View();
        }

        TempData["success"] = "Category deleted.";
        return RedirectToAction("index", "category");
    }
}
