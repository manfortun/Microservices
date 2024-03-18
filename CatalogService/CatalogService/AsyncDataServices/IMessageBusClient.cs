namespace CatalogService.AsyncDataServices;

public interface IMessageBusClient
{
    void SendMessageToAuth(string message);
}
