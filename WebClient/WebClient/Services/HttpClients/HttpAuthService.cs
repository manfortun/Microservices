using WebClient.Services.Interfaces;

namespace WebClient.Services.HttpClients;

public class HttpAuthService : IHttpClient
{
    public string Name => "AuthService";

    public string FriendlyName => "Authentication Service";
}
