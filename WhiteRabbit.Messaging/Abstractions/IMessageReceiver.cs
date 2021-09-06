using System;
using System.Threading.Tasks;

namespace WhiteRabbit.Messaging.Abstractions
{
    public interface IMessageReceiver
    {
        Task ReceiveAsync(IServiceProvider serviceProvider, object message);
    }

    public interface IMessageReceiver<T> : IMessageReceiver where T : class
    {
        Task ReceiveAsync(IServiceProvider serviceProvider, T message);
    }

    public abstract class MessageReceiver<T> : IMessageReceiver<T> where T : class
    {
        public Task ReceiveAsync(IServiceProvider serviceProvider, object message)
            => ReceiveAsync(serviceProvider, (T)message);

        public abstract Task ReceiveAsync(IServiceProvider serviceProvider, T message);
    }
}
