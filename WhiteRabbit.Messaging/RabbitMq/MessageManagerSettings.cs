using System.Text.Json;
using WhiteRabbit.Messaging.Abstractions;

namespace WhiteRabbit.Messaging.RabbitMq;

public class MessageManagerSettings
{
    public string ConnectionString { get; set; }

    public string ExchangeName { get; set; }

    public ushort QueuePrefetchCount { get; set; }

    public JsonSerializerOptions JsonSerializerOptions { get; set; } = JsonOptions.Default;
}
