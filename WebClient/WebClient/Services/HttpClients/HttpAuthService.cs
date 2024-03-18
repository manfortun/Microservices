using WebClient.Services.Interfaces;

namespace WebClient.Services.HttpClients;

public class HttpAuthService : IHttpService
{
    public string Name => "AuthService";

    public string FriendlyName => "Authentication Service";
}
