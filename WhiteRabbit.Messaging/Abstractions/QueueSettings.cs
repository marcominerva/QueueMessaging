using System;
using System.Collections.Generic;

namespace WhiteRabbit.Messaging.Abstractions
{
    public class QueueSettings
    {
        internal IDictionary<string, Type> Queues { get; } = new Dictionary<string, Type>();

        public void Add<T>(string queueName) where T : class
            => Queues.Add(queueName, typeof(T));
    }
}
