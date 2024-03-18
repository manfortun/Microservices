using CatalogService.DTOs;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;

namespace CatalogService.AsyncDataServices;

public class MessageBusClient
{
    private readonly IConfiguration _config;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public MessageBusClient(IConfiguration config)
    {
        _config = config;
        var factory = new ConnectionFactory()
        {
            HostName = _config["RabbitMQHost"],
            Port = int.Parse(_config["RabbitMQPort"])
        };

        try
        {
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not connect to the message bus: {ex.Message}");
        }
    }

    public void SendMessageToCommerce(string message)
    {
        MessageDto dto = new MessageDto
        {
            Message = message,
            Event = "Message"
        };

        var httpContent = JsonSerializer.Serialize(dto);

        if (_connection.IsOpen)
        {
            Console.WriteLine("RabbitMQ Connection Open; Sending message...");
            SendMessage(httpContent);
        }
        else
        {
            Console.WriteLine("RabbitMQ connection is closed; Unable to send message.");
        }
    }

    private void SendMessage(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(
            exchange: "trigger",
            routingKey: "",
            basicProperties: null,
            body: body);

        Console.WriteLine($"We have sent \"{message}\"");
    }

    public void Dispose()
    {
        Console.WriteLine("MessageBus disposed.");

        if (_channel.IsOpen)
        {
            _channel.Close();
            _connection.Close();
        }
    }

    private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
    {
        Console.WriteLine("RabbitMQ Connection Shutdown.");
    }
}
