﻿using AuthService.Models;
using AuthService.Services;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly JwtService _jwtService;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly ValidationService _validationService;

    public AuthController(
        SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        JwtService jwtService,
        IMapper mapper,
        IHttpContextAccessor contextAccessor,
        ValidationService validationService)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtService = jwtService;
        _mapper = mapper;
        _contextAccessor = contextAccessor;
        _validationService = validationService;
    }

    [HttpGet]
    public IActionResult TestConnection()
    {
        return Ok();
    }

    [HttpGet(nameof(GetId))]
    public ActionResult<string> GetId(string token)
    {
        string splitToken = token.ToString().Split(' ', 2)[1];

        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(splitToken);

        if (jsonToken is null)
        {
            return Unauthorized();
        }

        string id = jsonToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? default!;
        return id;
    }

    [HttpPost(nameof(ChangePassword))]
    public async Task<ActionResult<string>> ChangePassword(PasswordChangeData passwordChange, string ownerId)
    {
        bool isValid = _validationService.IsValid(passwordChange, out List<ValidationResult> validationResults);

        if (!isValid)
        {
            return BadRequest(validationResults.FirstOrDefault()?.ErrorMessage);
        }

        var user = await _userManager.FindByIdAsync(ownerId);

        var changePasswordResult = await _userManager.ChangePasswordAsync(user, passwordChange.Password, passwordChange.NewPassword);

        if (changePasswordResult.Succeeded)
        {
            return Ok("Password changed successfully");
        }
        else
        {
            return BadRequest(changePasswordResult.Errors.FirstOrDefault()?.Description);
        }
    }

    [HttpPost(nameof(Login))]
    public async Task<ActionResult> Login(LoginCredentials credentials)
    {
        bool isValid = _validationService.IsValid(credentials, out _);

        if (!isValid)
        {
            return BadRequest("The credentials provided is not valid.");
        }

        SignInResult result = await _signInManager.PasswordSignInAsync(
            credentials.Email,
            credentials.Password,
            isPersistent: true,
            lockoutOnFailure: false);

        if (result.Succeeded)
        {
            try
            {
                var identityUser = await _userManager.FindByEmailAsync(credentials.Email);
                var principal = await _signInManager.CreateUserPrincipalAsync(identityUser);

                string token = _jwtService.GenerateJwtToken(principal.Claims);
                return Ok(token);
            }
            catch
            {
                return BadRequest("Unable to generate token. Please contact admin.");
            }
        }

        return Unauthorized("Invalid email address or password.");
    }

    [HttpPost(nameof(Register))]
    public async Task<ActionResult<string>> Register(RegisterCredentials credentials)
    {
        bool isValid = _validationService.IsValid(credentials, out _);

        if (!isValid)
        {
            return BadRequest("The credentials provided is not valid.");
        }

        var identityUser = _mapper.Map<IdentityUser>(credentials);

        IdentityResult result = await _userManager
            .CreateAsync(
                user: identityUser,
                password: credentials.Password);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors.First().Description);
        }
        else
        {
            // add role to database if not exist
            await _roleManager.CreateAsync(new IdentityRole { Name = credentials.Role });

            // add user to role
            await _userManager.AddToRoleAsync(identityUser, credentials.Role);
        }

        return Ok("User registered successfully!");
    }

    [HttpGet("Info")]
    public IActionResult GetAuthorizationInfo()
    {
        return Ok();
    }
}
