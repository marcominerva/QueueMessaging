using System;
using Microsoft.Extensions.DependencyInjection;
using WhiteRabbit.Messaging.Abstractions;

namespace WhiteRabbit.Messaging
{
    public static class RabbitMQExtensions
    {
        public static IServiceCollection AddRabbitMq(this IServiceCollection services, Action<MessageManagerSettings> messageManagerConfiguration, Action<QueueSettings> queuesConfiguration)
        {
            services.AddSingleton<MessageManager>();
            services.AddSingleton<IMessageSender>(provider => provider.GetService<MessageManager>());

            var messageManagerSettings = new MessageManagerSettings();
            messageManagerConfiguration?.Invoke(messageManagerSettings);
            services.AddSingleton(messageManagerSettings);

            var queueSettings = new QueueSettings();
            queuesConfiguration?.Invoke(queueSettings);
            services.AddSingleton(queueSettings);

            return services;
        }

        public static IServiceCollection AddReceiver<TObject, TReceiver>(this IServiceCollection services) where TObject : class
            where TReceiver : class, IMessageReceiver<TObject>
        {
            services.AddHostedService<QueueListener>();
            services.AddSingleton<IMessageReceiver, TReceiver>();

            return services;
        }
    }
}
