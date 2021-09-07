using System.Threading.Tasks;
using WhiteRabbit.Messaging.Abstractions;
using WhiteRabbit.Shared;

namespace WhiteRabbit.Receivers
{
    public class InvoiceMessageReceiver : IMessageReceiver<Invoice>
    {
        public InvoiceMessageReceiver()
        {
        }

        public Task ReceiveAsync(Invoice message)
        {
            return Task.CompletedTask;
        }
    }
}
