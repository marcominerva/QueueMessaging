using System.Text;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WhiteRabbit.Messaging.Abstractions;

namespace WhiteRabbit.Messaging.ServiceBus;

internal class QueueListener<T> : BackgroundService, IAsyncDisposable where T : class
{
    private readonly MessageManager messageManager;
    private readonly MessageManagerSettings messageManagerSettings;
    private readonly ILogger logger;
    private readonly IServiceProvider serviceProvider;
    private readonly string queueName;

    private ServiceBusReceiver serviceBusReceiver;

    public QueueListener(MessageManager messageManager, MessageManagerSettings messageManagerSettings, QueueSettings settings, ILogger<QueueListener<T>> logger, IServiceProvider serviceProvider)
    {
        this.messageManager = messageManager;
        this.messageManagerSettings = messageManagerSettings;
        this.logger = logger;
        this.serviceProvider = serviceProvider;

        queueName = settings.Queues.First(q => q.Type == typeof(T)).Name;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogDebug("ServiceBus Listener for {QueueName} started", queueName);

        return base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogDebug("ServiceBus Listener for {QueueName} stopped", queueName);

        return base.StopAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        await messageManager.CreateQueueAsync(queueName);
        serviceBusReceiver = messageManager.ServiceBus.CreateReceiver(queueName);

        while (!stoppingToken.IsCancellationRequested)
        {
            var message = await serviceBusReceiver.ReceiveMessageAsync(cancellationToken: stoppingToken);
            if (message is not null)
            {
                try
                {
                    logger.LogDebug("Messaged received: {Request}", Encoding.UTF8.GetString(message.Body));

                    using var scope = serviceProvider.CreateScope();

                    var receiver = scope.ServiceProvider.GetService<IMessageReceiver<T>>();
                    var response = JsonSerializer.Deserialize<T>(message.Body, messageManagerSettings.JsonSerializerOptions ?? JsonOptions.Default);
                    await receiver.ReceiveAsync(response, stoppingToken);

                    logger.LogDebug("Message processed");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Unexpected error while processing message");
                }
                finally
                {
                    await serviceBusReceiver.CompleteMessageAsync(message, stoppingToken);
                }
            }
        }
    }

    public override void Dispose()
    {
        base.Dispose();

        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            await serviceBusReceiver.DisposeAsync();
        }
        catch
        {
        }
    }
}
