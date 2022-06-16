using System.Text.Json;
using WhiteRabbit.Messaging.Abstractions;

namespace WhiteRabbit.Messaging.ServiceBus;

public class MessageManagerSettings
{
    public string ConnectionString { get; set; }

    public JsonSerializerOptions JsonSerializerOptions { get; set; } = JsonOptions.Default;
}
