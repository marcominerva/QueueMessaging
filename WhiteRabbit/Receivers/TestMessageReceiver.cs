using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WhiteRabbit.Messaging.Abstractions;
using WhiteRabbit.Shared;

namespace WhiteRabbit.Receivers
{
    public class TestMessageReceiver : IMessageReceiver<Test>
    {
        private readonly ILogger<TestMessageReceiver> logger;

        public TestMessageReceiver(ILogger<TestMessageReceiver> logger)
        {
            this.logger = logger;
        }

        public async Task ReceiveAsync(Test message)
        {
            logger.LogInformation("Inizio elaborazione...");
            await Task.Delay(5000);
            logger.LogInformation("Fine elaborazione");
        }
    }
}
