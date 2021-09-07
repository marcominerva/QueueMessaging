using Microsoft.Extensions.DependencyInjection;
using WhiteRabbit.Messaging.Abstractions;

namespace WhiteRabbit.Messaging
{
    internal class RabbitMqMessagingBuilder : IMessagingBuilder
    {
        public IServiceCollection Services { get; }

        public RabbitMqMessagingBuilder(IServiceCollection services)
        {
            Services = services;
        }
    }
}
