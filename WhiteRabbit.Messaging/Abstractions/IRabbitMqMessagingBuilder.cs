using Microsoft.Extensions.DependencyInjection;

namespace WhiteRabbit.Messaging.Abstractions
{
    public interface IRabbitMqMessagingBuilder
    {
        IServiceCollection Services { get; }
    }
}