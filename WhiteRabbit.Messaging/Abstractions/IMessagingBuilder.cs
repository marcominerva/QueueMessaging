using Microsoft.Extensions.DependencyInjection;

namespace WhiteRabbit.Messaging.Abstractions;

public interface IMessagingBuilder
{
    IServiceCollection Services { get; }
}
