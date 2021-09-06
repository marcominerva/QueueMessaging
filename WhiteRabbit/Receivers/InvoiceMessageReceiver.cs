using System.Threading.Tasks;
using WhiteRabbit.Messaging.Abstractions;
using WhiteRabbit.Shared;

namespace WhiteRabbit.Receivers
{
    public class InvoiceMessageReceiver : MessageReceiver<Invoice>
    {
        public InvoiceMessageReceiver()
        {
        }

        public override Task ReceiveAsync(Invoice message)
        {
            return Task.CompletedTask;
        }
    }
}
