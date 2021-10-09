using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using WhiteRabbit.Messaging.RabbitMq;
using WhiteRabbit.Receivers;
using WhiteRabbit.Shared;

namespace WhiteRabbit
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "WhiteRabbit", Version = "v1" });
            });

            services.AddRabbitMq(settings =>
            {
                settings.ConnectionString = Configuration.GetConnectionString("RabbitMQ");
                settings.ExchangeName = Configuration.GetValue<string>("AppSettings:ApplicationName");
                settings.QueuePrefetchCount = Configuration.GetValue<ushort>("AppSettings:QueuePrefetchCount"); ;
            }, queues =>
            {
                queues.Add<Order>();
                queues.Add<Invoice>();
            })
            .AddReceiver<Order, OrderMessageReceiver>()
            .AddReceiver<Invoice, InvoiceMessageReceiver>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
    }
}
