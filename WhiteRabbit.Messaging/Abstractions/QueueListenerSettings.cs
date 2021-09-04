using System;
using System.Collections.Generic;

namespace WhiteRabbit.Messaging.Abstractions
{
    public class QueueListenerSettings
    {
        public IDictionary<string, Type> Queues { get; } = new Dictionary<string, Type>();

        public void Bind<T>(string queueName) where T : class
            => Queues.Add(queueName, typeof(T));
    }
}
