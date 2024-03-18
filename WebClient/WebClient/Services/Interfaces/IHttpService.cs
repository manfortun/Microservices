namespace WebClient.Services.Interfaces;

public interface IHttpService
{
    /// <summary>
    /// Connection name of the service
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Friendly name of the service
    /// </summary>
    string FriendlyName { get; }
}
