using System.Threading.Tasks;
using WhiteRabbit.Messaging.Abstractions;
using WhiteRabbit.Shared;

namespace WhiteRabbit.Receivers
{
    public class TestMessageReceiver : IMessageReceiver<Test>
    {
        public TestMessageReceiver()
        {
        }

        public Task ReceiveAsync(Test message)
        {
            return Task.CompletedTask;
        }
    }
}
