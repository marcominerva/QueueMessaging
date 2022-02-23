namespace WhiteRabbit.Messaging.Abstractions;

public class QueueSettings
{
    internal IDictionary<string, Type> Queues { get; } = new Dictionary<string, Type>();

    public void Add<T>(string queueName = null) where T : class
    {
        var type = typeof(T);
        Queues.Add(queueName ?? type.FullName, type);
    }
}
