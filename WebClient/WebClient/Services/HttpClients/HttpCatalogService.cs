using WebClient.Services.Interfaces;

namespace WebClient.Services.HttpClients;

public class HttpCatalogService : IHttpClient
{
    public string Name => "CatalogService";

    public string FriendlyName => "Catalog Service";
}
