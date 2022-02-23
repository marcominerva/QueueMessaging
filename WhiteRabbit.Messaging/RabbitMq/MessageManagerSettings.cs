namespace WhiteRabbit.Messaging.RabbitMq;

public class MessageManagerSettings
{
    public string ConnectionString { get; set; }

    public string ExchangeName { get; set; }

    public ushort QueuePrefetchCount { get; set; } = 0;
}
