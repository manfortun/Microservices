using System.Text.Json;

namespace AuthService.EventProcessing;

public class EventProcessor : IEventProcessor
{
    private readonly IServiceScopeFactory _scopeFactory;

    enum EventType
    {
        Message,
        Undetermined
    }

    public EventProcessor(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public void ProcessEvent(string message)
    {
        var eventType = DetermineEvent(message);

        switch (eventType)
        {
            case EventType.Message:
                PrintMessage(message);
                break;
            default:
                break;
        }
    }

    private EventType DetermineEvent(string message)
    {
        Console.WriteLine("Determining the event");

        var eventType = JsonSerializer.Deserialize<GenericEventDto>(message);

        switch (eventType.Event)
        {
            case nameof(EventType.Message):
                Console.WriteLine("Message detected");
                return EventType.Message;
            default:
                Console.WriteLine("Event type undetermined");
                return EventType.Undetermined;
        }
    }

    private void PrintMessage(string jsonMessage)
    {
        var message = JsonSerializer.Deserialize<Message>(jsonMessage);

        Console.WriteLine(message.Content);
    }
}
