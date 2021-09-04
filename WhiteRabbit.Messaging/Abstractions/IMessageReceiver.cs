using System.Threading.Tasks;

namespace WhiteRabbit.Messaging.Abstractions
{
    public interface IMessageReceiver
    {
        Task ReceiveAsync(object message);
    }

    public interface IMessageReceiver<T> : IMessageReceiver where T : class
    {
        Task ReceiveAsync(T message);
    }

    public abstract class MessageReceiver<T> : IMessageReceiver<T> where T : class
    {
        public Task ReceiveAsync(object message)
            => ReceiveAsync((T)message);

        public abstract Task ReceiveAsync(T message);
    }
}
