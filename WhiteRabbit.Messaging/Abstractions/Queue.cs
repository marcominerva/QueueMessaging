namespace WhiteRabbit.Messaging.Abstractions
{
    public class Queue
    {
        public string QueueName { get; }

        public string RoutingKey { get; }

        public Queue(string queueName, string routingKey)
            => (QueueName, RoutingKey) = (queueName, routingKey);
    }
}
