using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using WebClient.Services.Interfaces;

namespace WebClient.Services;

public class HttpService<T> : IHttpServiceWrapper where T : IHttpService
{
    private readonly T _client = Activator.CreateInstance<T>();
    private readonly IConfiguration _config;
    private readonly AuthService _authService;

    public IHttpService Service => _client;

    public HttpService(IConfiguration config, AuthService authService)
    {
        _config = config;
        _authService = authService;
    }

    public virtual async Task<HttpResponseMessage?> GetAsync(HttpContext context, params string[] endpoints)
    {
        string uri = $"{this}/{string.Join('/', endpoints)}";

        return await GetAsync(context, uri);
    }

    protected async Task<HttpResponseMessage?> GetAsync(HttpContext context, string absoluteUri)
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {
                string scheme = _authService.Scheme;
                string? token = _authService.GetToken(context);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, token);

                return await client.GetAsync(absoluteUri);
            }
        }
        catch
        {
            Console.WriteLine($"Unable to connect to {_client.FriendlyName}");
        }

        return default!;
    }

    public virtual async Task<HttpResponseMessage?> PostAsync(HttpContext context, object content, params string[] endpoints)
    {
        var httpContent = new StringContent(
            JsonSerializer.Serialize(content),
            Encoding.UTF8, "application/json");

        string uri = $"{this}/{string.Join('/', endpoints)}";

        return await PostAsync(context, httpContent, absoluteUri: uri);
    }

    protected async Task<HttpResponseMessage?> PostAsync(HttpContext context, StringContent? httpContent, string absoluteUri)
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {
                string scheme = _authService.Scheme;
                string? token = _authService.GetToken(context);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, token);

                return await client.PostAsync(absoluteUri, httpContent);
            }
        }
        catch
        {
            Console.WriteLine($"Unable to connect to {_client.FriendlyName}");
        }

        return default!;
    }

    public virtual async Task<HttpResponseMessage?> DeleteAsync(HttpContext context, params string[] endpoints)
    {
        string uri = $"{this}/{string.Join('/', endpoints)}";

        return await DeleteAsync(context, uri);
    }

    protected async Task<HttpResponseMessage?> DeleteAsync(HttpContext context, string absoluteUri)
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {
                string scheme = _authService.Scheme;
                string? token = _authService.GetToken(context);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, token);

                return await client.DeleteAsync(absoluteUri);
            }
        }
        catch
        {
            Console.WriteLine($"Unable to connect to {_client.FriendlyName}");
        }

        return default!;
    }

    public virtual IHttpServiceWrapper AddParameter(string name, object value)
    {
        var parameterizedService = new ParameterizedHttpService<T>(_config, _authService);
        parameterizedService.AddParameter(name, value);

        return parameterizedService;
    }

    public async Task<bool> IsRunning()
    {
        try
        {
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(ToString()))
            {
                return response.IsSuccessStatusCode;
            }
        }
        catch
        {
            return false;
        }
    }

    public override string ToString()
    {
        return _config.GetConnectionString(_client.Name) ??
            throw new NotImplementedException($"No connection string for {_client.FriendlyName}");
    }
}
