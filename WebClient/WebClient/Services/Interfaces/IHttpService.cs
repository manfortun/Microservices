namespace WebClient.Services.Interfaces;

public interface IHttpService
{
    IHttpClient Client { get; }
    Task<HttpResponseMessage?> PostAsync(HttpContext context, object content, params string[] endpoints);
    Task<HttpResponseMessage?> GetAsync(HttpContext context, params string[] endpoints);
    Task<HttpResponseMessage?> DeleteAsync(HttpContext context, params string[] endpoints);
    IHttpService AddParameter(string name, object value);
    Task<bool> IsRunning();
}
