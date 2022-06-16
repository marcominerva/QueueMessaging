using Microsoft.Extensions.DependencyInjection;
using WhiteRabbit.Messaging.Abstractions;

namespace WhiteRabbit.Messaging.RabbitMq;

public static class RabbitMQExtensions
{
    public static IMessagingBuilder AddRabbitMq(this IServiceCollection services, Action<MessageManagerSettings> messageManagerConfiguration, Action<QueueSettings> queuesConfiguration)
    {
        services.AddSingleton<MessageManager>();
        services.AddSingleton<IMessageSender>(provider => provider.GetService<MessageManager>());

        var messageManagerSettings = new MessageManagerSettings();
        messageManagerConfiguration.Invoke(messageManagerSettings);
        services.AddSingleton(messageManagerSettings);

        var queueSettings = new QueueSettings();
        queuesConfiguration.Invoke(queueSettings);
        services.AddSingleton(queueSettings);

        return new DefaultMessagingBuilder(services);
    }

    public static IMessagingBuilder AddReceiver<TObject, TReceiver>(this IMessagingBuilder builder) where TObject : class
        where TReceiver : class, IMessageReceiver<TObject>
    {
        builder.Services.AddHostedService<QueueListener<TObject>>();
        builder.Services.AddTransient<IMessageReceiver<TObject>, TReceiver>();

        return builder;
    }
}
