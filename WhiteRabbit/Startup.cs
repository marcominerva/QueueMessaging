using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using WhiteRabbit.Messaging;
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
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WhiteRabbit", Version = "v1" });
            });

            services.AddRabbitMq(settings =>
            {
                settings.ConnectionString = "amqp://guest:guest@pi4dev:5672";
                settings.ExchangeName = "my-app";
                settings.QueuePrefetchCount = 0;
            }, queues =>
            {
                queues.Add<Test>();
                queues.Add<Invoice>();
            })
            .AddReceiver<Test, TestMessageReceiver>()
            .AddReceiver<Invoice, InvoiceMessageReceiver>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WhiteRabbit v1"));
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
