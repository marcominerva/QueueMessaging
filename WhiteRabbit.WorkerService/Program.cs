using WhiteRabbit.Messaging.RabbitMq;
using WhiteRabbit.Receivers;
using WhiteRabbit.Shared;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(ConfigureServices)
    .Build();

await host.RunAsync();


void ConfigureServices(HostBuilderContext hostingContext, IServiceCollection services)
{
    services.AddRabbitMq(settings =>
    {
        settings.ConnectionString = hostingContext.Configuration.GetConnectionString("RabbitMQ");
        settings.ExchangeName = hostingContext.Configuration.GetValue<string>("AppSettings:ApplicationName");
        settings.QueuePrefetchCount = hostingContext.Configuration.GetValue<ushort>("AppSettings:QueuePrefetchCount"); ;
    }, queues =>
    {
        queues.Add<Order>();
        queues.Add<Invoice>();
    })
    .AddReceiver<Order, OrderMessageReceiver>()
    .AddReceiver<Invoice, InvoiceMessageReceiver>();

    //services.AddServiceBus(settings =>
    //{
    //    settings.ConnectionString = hostContext.Configuration.GetConnectionString("ServiceBus");
    //}, queues =>
    //{
    //    queues.Add<Order>();
    //    queues.Add<Invoice>();
    //})
    //.AddReceiver<Order, OrderMessageReceiver>()
    //.AddReceiver<Invoice, InvoiceMessageReceiver>();    })
}