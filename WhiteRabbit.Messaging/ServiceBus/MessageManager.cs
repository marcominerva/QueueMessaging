using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WhiteRabbit.Messaging.Abstractions;

namespace WhiteRabbit.Messaging.ServiceBus
{
    internal class MessageManager : IMessageSender, IAsyncDisposable
    {
        internal ServiceBusClient ServiceBus { get; private set; }

        private readonly ServiceBusAdministrationClient administrationClient;
        private readonly Lazy<Task<Dictionary<string, ServiceBusSender>>> senders;

        private readonly MessageManagerSettings messageManagerSettings;
        private readonly QueueSettings queueSettings;

        private static readonly JsonSerializerOptions jsonSerializerOptions = new(JsonSerializerDefaults.Web)
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
        };

        public MessageManager(MessageManagerSettings messageManagerSettings, QueueSettings queueSettings)
        {
            ServiceBus = new(messageManagerSettings.ConnectionString);
            administrationClient = new(messageManagerSettings.ConnectionString);

            senders = new(async () =>
            {
                var senders = new Dictionary<string, ServiceBusSender>();

                var allQueues = await administrationClient.GetQueuesAsync().ToListAsync();
                foreach (var queue in queueSettings.Queues.Keys)
                {
                    var foundQueue = allQueues.FirstOrDefault(q => q.Name.ToUpperInvariant() == queue.ToUpperInvariant());
                    if (foundQueue == null)
                    {
                        await administrationClient.CreateQueueAsync(queue);
                    }

                    var sender = ServiceBus.CreateSender(queue);
                    senders.Add(queue, sender);
                }

                return senders;
            });

            this.messageManagerSettings = messageManagerSettings;
            this.queueSettings = queueSettings;
        }

        public async Task CreateQueueAsync(string queueName)
        {
            var queueExists = await administrationClient.QueueExistsAsync(queueName);
            if (!queueExists)
            {
                await administrationClient.CreateQueueAsync(queueName);
            }
        }

        private async Task PublishAsync(ReadOnlyMemory<byte> body, string routingKey, int priority = 1)
        {
            var senderList = await senders.Value;
            var sender = senderList[routingKey];
            await sender.SendMessageAsync(new ServiceBusMessage(body));
        }

        public Task PublishAsync<T>(T message, int priority = 1) where T : class
        {
            var sendBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message, jsonSerializerOptions));

            var routingKey = queueSettings.Queues.First(q => q.Value == typeof(T)).Key;
            return PublishAsync(sendBytes.AsMemory(), routingKey, priority);
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                await ServiceBus.DisposeAsync();
            }
            catch
            {
            }

            GC.SuppressFinalize(this);
        }
    }
}
