using Microsoft.Extensions.Hosting;
using WhiteRabbit.Messaging;
using WhiteRabbit.Receivers;
using WhiteRabbit.Shared;

namespace WhiteRabbit.WorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddRabbitMq(settings =>
                    {
                        settings.ConnectionString = "amqp://guest:guest@pi4dev:5672";
                        settings.ExchangeName = "my-app";
                        settings.QueuePrefetchCount = 2;
                    }, queues =>
                    {
                        queues.Add<Test>();
                        queues.Add<Invoice>();
                    })
                    .AddReceiver<Test, TestMessageReceiver>()
                    .AddReceiver<Invoice, InvoiceMessageReceiver>();
                });
    }
}
