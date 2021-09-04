using System.Collections.Generic;

namespace WhiteRabbit.Messaging.Abstractions
{
    public class MessageManagerSettings
    {
        public string ConnectionString { get; set; }

        public string ExchangeName { get; set; }

        public ushort QueuePrefetchCount { get; set; } = 0;

        public IList<Queue> Queues { get; } = new List<Queue>();

        public void AddQueue(string queueName, string routingKey = null) => Queues.Add(new Queue(queueName, routingKey));
    }
}
