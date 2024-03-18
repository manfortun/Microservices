using WebClient.Services.Interfaces;

namespace WebClient.Services.HttpClients;

public class HttpCatalogService : IHttpService
{
    public string Name => "CatalogService";

    public string FriendlyName => "Catalog Service";
}
