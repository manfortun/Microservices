using System.Text.Json;
using System.Text;

namespace CatalogService.SyncDataServices.Http;

public class HttpCommandDataClient : ICommandDataClient
{
    private readonly IConfiguration _config;

    public HttpCommandDataClient(IConfiguration config)
    {
        _config = config;
    }

    public async Task<string> GetId(string token)
    {
        using(HttpClient client = new HttpClient())
        {
            var response = await client.GetAsync($"{_config.GetConnectionString("AuthServiceConnection")}/GetId?token={token}");

            if (response.IsSuccessStatusCode)
            {
                string id = await response.Content.ReadAsStringAsync();
                return id;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
