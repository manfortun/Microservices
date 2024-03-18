using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebClient.DTOs;
using WebClient.Models;
using WebClient.Services;
using WebClient.Services.HttpClients;
using WebClient.Services.Interfaces;

namespace WebClient.Controllers;

[AllowAnonymous]
public class AccountController : Controller
{
    private readonly IMapper _mapper;
    private readonly IHttpServiceWrapper _httpService;
    private readonly AuthService _authService;

    public AccountController(
        IMapper mapper,
        IConfiguration config,
        AuthService authService)
    {
        _mapper = mapper;
        _authService = authService;
        _httpService = new HttpService<HttpAuthService>(config, authService);
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterCredentials credentials)
    {
        var credentialsDto = _mapper.Map<RegisterCredentialsDto>(credentials);
        HttpResponseMessage? response = await _httpService.PostAsync(HttpContext, credentialsDto, "Register");

        if (response is null)
        {
            ModelState.AddModelError("", "Unable to connect to authentication service. Please contact admin.");
            return View();
        }

        return await Login(_mapper.Map<LogInCredentials>(credentials));
    }

    [HttpGet]
    public async Task<IActionResult> Login()
    {
        if (User?.Identity?.IsAuthenticated == true)
        {
            await _authService.RefreshTokenAsync(HttpContext);
            return RedirectToAction("Index", "Home");
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LogInCredentials credentials)
    {
        HttpResponseMessage? response = await _httpService.PostAsync(HttpContext, credentials, "Login");

        if (response is null)
        {
            ModelState.AddModelError("", "Unable to connect to authentication service. Please contact admin.");
            return View();
        }

        if (response.IsSuccessStatusCode)
        {
            string token = await response.Content.ReadAsStringAsync();

            await _authService.SignInAsync(HttpContext, token);

            return RedirectToAction("Index", "Home");
        }
        else
        {
            string message = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", message);
            return View();
        }
    }

    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(PasswordChangeDto passwordChange)
    {
        HttpResponseMessage? response = await _httpService
            .PostAsync(HttpContext, passwordChange, "ChangePassword");

        var content = await HttpContentDeserializer.Deserialize<string>(response);

        if (!content.Connected)
        {
            ModelState.AddModelError("", "Password change failed. Try again later.");
            return View();
        }

        if (!content.Successful)
        {
            ModelState.AddModelError("", content.Content);
            return View();
        }

        TempData["success"] = "Password changed successfully.";
        return View();
    }

    public async Task<IActionResult> Logout()
    {
        await _authService.SignOutAsync(HttpContext);
        return RedirectToAction(nameof(Login));
    }
}
