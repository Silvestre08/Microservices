using MassTransit;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using System.Threading.Tasks;

namespace AdminNotification.Worker
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddMassTransit(x =>
                    {
                        x.SetKebabCaseEndpointNameFormatter();

                        var entryAssembly = Assembly.GetEntryAssembly();
                        x.AddConsumers(entryAssembly);

                        x.UsingRabbitMq((context, cfg) => 
                        {
                            cfg.ReceiveEndpoint("order-created", e => 
                            { 
                                e.ConfigureConsumer<OrderCreatedNotification>(context); 
                            });
                            cfg.ConfigureEndpoints(context); 
                        });
                    });
                });
    }
}
