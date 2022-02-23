namespace WhiteRabbit.Messaging.Abstractions;

public interface IMessageReceiver<T> where T : class
{
    Task ReceiveAsync(T message);
}
