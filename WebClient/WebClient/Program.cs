using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebClient.Services;
using WebClient.Services.HttpClients;
using WebClient.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddTransient<AuthService>();
builder.Services.AddTransient<IUploadService<IFormFile>, FormFileUploadService>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
    })
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration.GetConnectionString("AuthService");
        options.Audience = builder.Configuration["Jwt:Issuer"];
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("mysecretkey")),
            ValidateIssuer = true,
            ValidateAudience = true,
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddMvc(options =>
{
    var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();

    options.Filters.Add(new AuthorizeFilter(policy));
}).AddXmlSerializerFormatters();

var app = builder.Build();

Console.WriteLine(AppDomain.CurrentDomain.FriendlyName);
Console.WriteLine("-----------------------------------------------");
Console.WriteLine("Connection Strings:");

using (var serviceScope = builder.Services.BuildServiceProvider().CreateScope())
{
    var authService = serviceScope.ServiceProvider.GetService<AuthService>();

    List<IHttpServiceWrapper> httpServices = new List<IHttpServiceWrapper>
    {
        new HttpService<HttpAuthService>(builder.Configuration, authService),
        new HttpService<HttpCatalogService>(builder.Configuration, authService),
    };

    foreach (var service in httpServices)
    {
        string serviceStatus = await service.IsRunning() ? "UP" : "DOWN";
        Console.WriteLine($"\t{service.Service.FriendlyName} ({service}): {serviceStatus}");
    }
}
Console.WriteLine("-----------------------------------------------");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
