using Microsoft.Extensions.Configuration;
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
                        settings.ConnectionString = hostContext.Configuration.GetConnectionString("RabbitMQ");
                        settings.ExchangeName = hostContext.Configuration.GetValue<string>("AppSettings:ApplicationName");
                        settings.QueuePrefetchCount = hostContext.Configuration.GetValue<ushort>("AppSettings:QueuePrefetchCount"); ;
                    }, queues =>
                    {
                        queues.Add<Order>();
                        queues.Add<Invoice>();
                    })
                    .AddReceiver<Order, OrderMessageReceiver>()
                    .AddReceiver<Invoice, InvoiceMessageReceiver>();
                });
    }
}
