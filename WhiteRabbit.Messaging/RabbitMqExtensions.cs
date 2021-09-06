using System;
using Microsoft.Extensions.DependencyInjection;
using WhiteRabbit.Messaging.Abstractions;

namespace WhiteRabbit.Messaging
{
    public static class RabbitMQExtensions
    {
        public static IRabbitMqMessagingBuilder AddRabbitMq(this IServiceCollection services, Action<MessageManagerSettings> messageManagerConfiguration, Action<QueueSettings> queuesConfiguration)
        {
            services.AddSingleton<MessageManager>();
            services.AddSingleton<IMessageSender>(provider => provider.GetService<MessageManager>());

            var messageManagerSettings = new MessageManagerSettings();
            messageManagerConfiguration?.Invoke(messageManagerSettings);
            services.AddSingleton(messageManagerSettings);

            var queueSettings = new QueueSettings();
            queuesConfiguration?.Invoke(queueSettings);
            services.AddSingleton(queueSettings);

            return new RabbitMqMessagingBuilder(services);
        }

        public static IRabbitMqMessagingBuilder AddReceiver<TObject, TReceiver>(this IRabbitMqMessagingBuilder builder) where TObject : class
            where TReceiver : class, IMessageReceiver<TObject>
        {
            builder.Services.AddHostedService<QueueListener>();
            builder.Services.AddSingleton<IMessageReceiver, TReceiver>();

            return builder;
        }
    }
}
