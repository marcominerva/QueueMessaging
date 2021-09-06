using System.Threading.Tasks;
using WhiteRabbit.Messaging.Abstractions;
using WhiteRabbit.Shared;

namespace WhiteRabbit.Receivers
{
    public class TestMessageReceiver : MessageReceiver<Test>
    {
        public TestMessageReceiver()
        {
        }

        public override Task ReceiveAsync(Test message)
        {
            return Task.CompletedTask;
        }
    }
}
