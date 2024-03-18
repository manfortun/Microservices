namespace AuthService.EventProcessing;

public interface IEventProcessor
{
    void ProcessEvent(string message);
}
