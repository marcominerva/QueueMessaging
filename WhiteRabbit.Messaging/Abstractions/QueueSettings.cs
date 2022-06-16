namespace WhiteRabbit.Messaging.Abstractions;

public class QueueSettings
{
    internal IList<(string Name, Type Type)> Queues { get; } = new List<(string, Type)>();

    public void Add<T>(string queueName = null) where T : class
    {
        var type = typeof(T);
        Queues.Add((queueName ?? type.FullName, type));
    }
}
