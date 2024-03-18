﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace WebClient.Services;

public class AuthService
{
    private const string TOKEN_NAME = "login";
    public string Scheme => CookieAuthenticationDefaults.AuthenticationScheme;

    /// <summary>
    /// Sets the <paramref name="token"/> into the <paramref name="context"/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="token"></param>
    public async Task SignInAsync(HttpContext context, string token)
    {
        context.Response.Cookies.Append(TOKEN_NAME, token);

        var userIdentity = new ClaimsIdentity(GetClaims(token), Scheme);

        var principal = new ClaimsPrincipal(userIdentity);

        await context.SignInAsync(Scheme, principal);
    }

    /// <summary>
    /// Signs the current user out
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task SignOutAsync(HttpContext context)
    {
        await context.SignOutAsync(Scheme);

        context.Response.Cookies.Delete(TOKEN_NAME);
    }

    /// <summary>
    /// Determines if a user is authorized
    /// </summary>
    /// <param name="context"></param>
    /// <param name="roleName"></param>
    /// <returns></returns>
    public async Task<bool> IsAuthorizedAsync(HttpContext context, string roleName = "")
    {
        if (context is null)
        {
            return false;
        }

        string? token = GetToken(context);
        bool isAuthorized = !string.IsNullOrEmpty(token);

        if (isAuthorized)
        {
            await RefreshTokenAsync(context);

            if (!string.IsNullOrEmpty(roleName))
            {
                isAuthorized &= HasRole(token, roleName);
            }
        }

        return isAuthorized;
    }

    /// <summary>
    /// Obtains the claims of the <paramref name="token"/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public static IEnumerable<Claim> GetClaims(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.ReadJwtToken(token);

        return securityToken.Claims;
    }

    /// <summary>
    /// Refreshes the expiration of current token
    /// </summary>
    /// <param name="context"></param>
    public async Task RefreshTokenAsync(HttpContext context)
    {
        string? token = GetToken(context);

        if (!string.IsNullOrEmpty(token))
        {
            await SignInAsync(context, token);
        }
    }

    /// <summary>
    /// Retrieves the token from the cookies
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public string? GetToken(HttpContext context)
    {
        return context.Request.Cookies[TOKEN_NAME];
    }

    /// <summary>
    /// Checks if the <paramref name="token"/> has the role of <paramref name="roleName"/>
    /// </summary>
    /// <param name="token"></param>
    /// <param name="roleName"></param>
    /// <returns></returns>
    private bool HasRole(string? token, string roleName)
    {
        if (string.IsNullOrEmpty(token))
        {
            return false;
        }

        var claims = GetClaims(token);

        var roleClaims = claims.Where(c => c.Type == ClaimTypes.Role);

        return roleClaims.Any(roles => roles.ValueType == roleName);
    }
}
