using System.Text;
using System.Text.Json;
using WebClient.Services.Interfaces;

namespace WebClient.Services;

public class ParameterizedHttpService<T> : HttpService<T> where T : IHttpClient
{
    private readonly Dictionary<string, object> _queryParameters = [];
    public ParameterizedHttpService(IConfiguration config, AuthService authService) : base(config, authService) { }

    public override async Task<HttpResponseMessage?> GetAsync(HttpContext context, params string[] endpoints)
    {
        string uri = BuildAbsoluteUri(endpoints);

        return await base.GetAsync(context, absoluteUri: uri);
    }

    public override async Task<HttpResponseMessage?> PostAsync(HttpContext context, object content, params string[] endpoints)
    {
        string uri = BuildAbsoluteUri(endpoints);

        var httpContent = new StringContent(
            JsonSerializer.Serialize(content),
            Encoding.UTF8, "application/json");

        return await PostAsync(context, httpContent, absoluteUri: uri);
    }

    public override async Task<HttpResponseMessage?> DeleteAsync(HttpContext context, params string[] endpoints)
    {
        string uri = BuildAbsoluteUri(endpoints);

        return await DeleteAsync(context, absoluteUri: uri);
    }

    public override ParameterizedHttpService<T> AddParameter(string name, object value)
    {
        _queryParameters.Add(name, value);
        return this;
    }

    private string BuildAbsoluteUri(params string[] endpoints)
    {
        string uri = ToString();

        if (endpoints.Any())
        {
            uri = $"{uri}/{string.Join('/', endpoints)}";
        }
        if (_queryParameters.Any())
        {
            string stringedQP = string.Join("&", _queryParameters.Select(qp => $"{qp.Key}={qp.Value}"));
            uri = $"{uri}?{stringedQP}";
        }

        return uri;
    }
}
