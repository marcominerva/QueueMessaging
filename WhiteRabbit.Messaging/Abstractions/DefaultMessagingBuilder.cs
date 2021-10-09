using Microsoft.Extensions.DependencyInjection;

namespace WhiteRabbit.Messaging.Abstractions
{
    internal class DefaultMessagingBuilder : IMessagingBuilder
    {
        public IServiceCollection Services { get; }

        public DefaultMessagingBuilder(IServiceCollection services)
        {
            Services = services;
        }
    }
}
