using WhiteRabbit.Messaging.RabbitMq;
using WhiteRabbit.Receivers;
using WhiteRabbit.Shared;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(ConfigureServices)
    .Build();

await host.RunAsync();

void ConfigureServices(HostBuilderContext hostingContext, IServiceCollection services)
{
    // Configures the application message receivers.
    var configuration = hostingContext.Configuration;

    services.AddRabbitMq(settings =>
    {
        settings.ConnectionString = configuration.GetConnectionString("RabbitMQ");
        settings.ExchangeName = configuration.GetValue<string>("AppSettings:ApplicationName");
        settings.QueuePrefetchCount = configuration.GetValue<ushort>("AppSettings:QueuePrefetchCount");
    }, queues =>
    {
        queues.Add<Order>();
        queues.Add<Invoice>();
    })
    .AddReceiver<Order, OrderMessageReceiver>()
    .AddReceiver<Invoice, InvoiceMessageReceiver>();

    //services.AddServiceBus(settings =>
    //{
    //    settings.ConnectionString = configuration.GetConnectionString("ServiceBus");
    //}, queues =>
    //{
    //    queues.Add<Order>();
    //    queues.Add<Invoice>();
    //})
    //.AddReceiver<Order, OrderMessageReceiver>()
    //.AddReceiver<Invoice, InvoiceMessageReceiver>();
}