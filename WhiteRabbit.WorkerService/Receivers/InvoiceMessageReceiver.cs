using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WhiteRabbit.Messaging.Abstractions;
using WhiteRabbit.Shared;

namespace WhiteRabbit.Receivers
{
    public class InvoiceMessageReceiver : IMessageReceiver<Invoice>
    {
        private readonly ILogger<InvoiceMessageReceiver> logger;

        public InvoiceMessageReceiver(ILogger<InvoiceMessageReceiver> logger)
        {
            this.logger = logger;
        }

        public async Task ReceiveAsync(Invoice message)
        {
            logger.LogInformation("Inizio elaborazione...");
            await Task.Delay(7000);
            logger.LogInformation("Fine elaborazione");
        }
    }
}
