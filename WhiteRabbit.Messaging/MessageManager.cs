using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using WhiteRabbit.Messaging.Abstractions;

namespace WhiteRabbit.Messaging
{
    internal class MessageManager : IMessageSender, IDisposable
    {
        private const string RetryAttemptsHeader = "x-retry-attempts";
        private const string MaxPriorityHeader = "x-max-priority";

        internal IConnection Connection { get; private set; }

        internal IModel Channel { get; private set; }

        private readonly MessageManagerSettings messageManagerSettings;
        private readonly QueueSettings queueSettings;

        private static readonly JsonSerializerOptions jsonSerializerOptions = new(JsonSerializerDefaults.Web)
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
        };

        public MessageManager(MessageManagerSettings messageManagerSettings, QueueSettings queueSettings)
        {
            var factory = new ConnectionFactory { Uri = new Uri(messageManagerSettings.ConnectionString) };
            Connection = factory.CreateConnection();

            Channel = Connection.CreateModel();

            if (messageManagerSettings.QueuePrefetchCount > 0)
            {
                Channel.BasicQos(0, messageManagerSettings.QueuePrefetchCount, false);
            }

            Channel.ExchangeDeclare(messageManagerSettings.ExchangeName, ExchangeType.Direct, durable: true);

            foreach (var queue in queueSettings.Queues.Keys)
            {
                var args = new Dictionary<string, object>
                {
                    [MaxPriorityHeader] = 10
                };

                Channel.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false, args);
                Channel.QueueBind(queue, messageManagerSettings.ExchangeName, queue, null);
            }

            this.messageManagerSettings = messageManagerSettings;
            this.queueSettings = queueSettings;
        }

        private Task PublishAsync(ReadOnlyMemory<byte> body, string routingKey, int priority = 1, int retryAttempts = 0)
        {
            var properties = Channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.Priority = Convert.ToByte(priority);
            properties.Headers = new Dictionary<string, object>
            {
                [RetryAttemptsHeader] = retryAttempts
            };

            Channel.BasicPublish(messageManagerSettings.ExchangeName, routingKey, properties, body);
            return Task.CompletedTask;
        }

        public Task PublishAsync<T>(T message, int priority = 1, int retryAttempts = 0) where T : class
        {
            var sendBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message, jsonSerializerOptions));

            var routingKey = queueSettings.Queues.First(q => q.Value == typeof(T)).Key;
            return PublishAsync(sendBytes.AsMemory(), routingKey, priority, retryAttempts);
        }

        public void MarkAsComplete(BasicDeliverEventArgs message) => Channel.BasicAck(message.DeliveryTag, false);

        public void MarkAsRejected(BasicDeliverEventArgs message, bool retry = false)
        {
            var remainingRetryAttempts = 0;
            if (retry && message.BasicProperties.Headers.TryGetValue(RetryAttemptsHeader, out var attempts) && attempts is int attemptsValue)
            {
                remainingRetryAttempts = attemptsValue - 1;
            }

            if (remainingRetryAttempts <= 0)
            {
                Channel.BasicReject(message.DeliveryTag, false);
            }
            else
            {
                Channel.BasicNack(message.DeliveryTag, false, false);
                PublishAsync(message.Body, message.RoutingKey, message.BasicProperties.Priority, remainingRetryAttempts);
            }
        }

        public void Dispose()
        {
            try
            {
                if (Channel.IsOpen)
                {
                    Channel.Close();
                }

                if (Connection.IsOpen)
                {
                    Connection.Close();
                }
            }
            catch
            {
            }

            GC.SuppressFinalize(this);
        }
    }
}
