using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using WhiteRabbit.Messaging.Abstractions;

namespace WhiteRabbit.Messaging.RabbitMq
{
    internal class QueueListener<T> : BackgroundService where T : class
    {
        private readonly MessageManager messageManager;
        private readonly ILogger logger;
        private readonly IServiceProvider serviceProvider;
        private readonly string queueName;

        private static readonly JsonSerializerOptions jsonSerializerOptions = new(JsonSerializerDefaults.Web)
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
        };

        public QueueListener(MessageManager messageManager, QueueSettings settings, ILogger<QueueListener<T>> logger, IServiceProvider serviceProvider)
        {
            this.messageManager = messageManager;
            this.logger = logger;
            this.serviceProvider = serviceProvider;

            queueName = settings.Queues.First(q => q.Value == typeof(T)).Key;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogDebug("RabbitMQ Listener for {QueueName} started", queueName);

            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogDebug("RabbitMQ Listener for {QueueName} stopped", queueName);

            return base.StopAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(messageManager.Channel);
            consumer.Received += async (_, message) =>
            {
                try
                {
                    logger.LogDebug("Messaged received: {Request}", Encoding.UTF8.GetString(message.Body.Span));

                    using var scope = serviceProvider.CreateScope();

                    var receiver = scope.ServiceProvider.GetService<IMessageReceiver<T>>();
                    var response = JsonSerializer.Deserialize<T>(message.Body.Span, jsonSerializerOptions);
                    await receiver.ReceiveAsync(response);

                    messageManager.MarkAsComplete(message);

                    logger.LogDebug("Message processed");
                }
                catch (Exception ex)
                {
                    messageManager.MarkAsRejected(message, retry: true);
                    logger.LogError(ex, "Unexpected error while processing message");
                }

                stoppingToken.ThrowIfCancellationRequested();
            };

            messageManager.Channel.BasicConsume(queueName, autoAck: false, consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            messageManager.Dispose();
            base.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
