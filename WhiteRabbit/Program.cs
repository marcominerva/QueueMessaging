using Microsoft.OpenApi.Models;
using WhiteRabbit.Messaging.ServiceBus;
using WhiteRabbit.Receivers;
using WhiteRabbit.Shared;

var builder = WebApplication.CreateBuilder(args);
ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();
Configure(app, app.Environment);

app.Run();

void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    services.AddControllers();
    services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "WhiteRabbit", Version = "v1" });
    });

    //services.AddRabbitMq(settings =>
    //{
    //    settings.ConnectionString = configuration.GetConnectionString("RabbitMQ");
    //    settings.ExchangeName = configuration.GetValue<string>("AppSettings:ApplicationName");
    //    settings.QueuePrefetchCount = configuration.GetValue<ushort>("AppSettings:QueuePrefetchCount");
    //}, queues =>
    //{
    //    queues.Add<Order>();
    //    queues.Add<Invoice>();
    //})
    //.AddReceiver<Order, OrderMessageReceiver>()
    //.AddReceiver<Invoice, InvoiceMessageReceiver>();

    services.AddServiceBus(settings =>
    {
        settings.ConnectionString = configuration.GetConnectionString("ServiceBus");
    }, queues =>
    {
        queues.Add<Order>();
        queues.Add<Invoice>();
    })
    .AddReceiver<Order, OrderMessageReceiver>()
    .AddReceiver<Invoice, InvoiceMessageReceiver>();
}

void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "WhiteRabbit v1"));
    }

    app.UseHttpsRedirection();

    app.UseRouting();

    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });
}