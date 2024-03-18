namespace WebClient.Services.Interfaces;

public interface IHttpServiceWrapper
{
    /// <summary>
    /// Client instance
    /// </summary>
    IHttpService Service { get; }

    /// <summary>
    /// Send a POST request to the service instance
    /// </summary>
    /// <param name="context"></param>
    /// <param name="content"></param>
    /// <param name="endpoints"></param>
    /// <returns></returns>
    Task<HttpResponseMessage?> PostAsync(HttpContext context, object content, params string[] endpoints);

    /// <summary>
    /// Send a GET request to the service instance
    /// </summary>
    /// <param name="context"></param>
    /// <param name="endpoints"></param>
    /// <returns></returns>
    Task<HttpResponseMessage?> GetAsync(HttpContext context, params string[] endpoints);

    /// <summary>
    /// Send a DELETE request to the service instance
    /// </summary>
    /// <param name="context"></param>
    /// <param name="endpoints"></param>
    /// <returns></returns>
    Task<HttpResponseMessage?> DeleteAsync(HttpContext context, params string[] endpoints);

    /// <summary>
    /// Add a query parameter to the request
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IHttpServiceWrapper AddParameter(string name, object value);

    /// <summary>
    /// Check if the service is running
    /// </summary>
    /// <returns></returns>
    Task<bool> IsRunning();
}
