using System;
using Microsoft.Extensions.DependencyInjection;
using WhiteRabbit.Messaging.Abstractions;

namespace WhiteRabbit.Messaging
{
    public static class RabbitMQExtensions
    {
        public static IServiceCollection AddRabbitMq(this IServiceCollection services, Action<MessageManagerSettings> configuration)
        {
            services.AddSingleton<MessageManager>();

            var settings = new MessageManagerSettings();
            configuration?.Invoke(settings);
            services.AddSingleton(settings);

            return services;
        }

        public static IServiceCollection AddListener(this IServiceCollection services, Action<QueueListenerSettings> configuration)
        {
            var settings = new QueueListenerSettings();
            configuration?.Invoke(settings);
            services.AddSingleton(settings);

            services.AddHostedService<QueueListener>();

            return services;
        }

        public static IServiceCollection AddReceiver<TObject, TReceiver>(this IServiceCollection services) where TObject : class
            where TReceiver : class, IMessageReceiver<TObject>
        {
            services.AddScoped<IMessageReceiver, TReceiver>();
            return services;
        }
    }
}
