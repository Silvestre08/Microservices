using Contracts.Filters;
using Contracts.Models;
using Contracts.Response;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Orders.Data;
using Orders.Domain;
using Orders.Service;
using OrdersApi.Consumer;
using OrdersApi.Infrastructure.Mappings;
using OrdersApi.Service.Clients;
using OrdersApi.Services;

namespace OrdersApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
            });

            builder.Services.AddAutoMapper(typeof(OrderProfileMapping));
            builder.Services.AddDbContext<OrderContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
                options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());

            });

            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IOrderService, OrderService>();

            builder.Services.AddHttpClient<IProductStockServiceClient, ProductStockServiceClient>();
            builder.Services.AddScoped<Tenant>();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddMassTransit(options =>
            {
                options.SetKebabCaseEndpointNameFormatter();
                //options.AddConsumer<VerifyOrderConsumer>();
                //options.AddConsumer<OrderCreateFaultConsumer>();
                //options.AddConsumer<OrderCreatedConsumer, OrderCreatedConsumerDefinition>();
                options.AddRequestClient<VerifyOrder>();
                options.AddEntityFrameworkOutbox<OrderContext>(o => 
                {
                    o.UseSqlServer();
                    o.UseBusOutbox();
                });
                options.UsingRabbitMq((context, config) =>
                {
                                        config.UseMessageRetry((r) => r.Immediate(2));
                    config.UseSendFilter(typeof(TenantSendFilter<>),context);
                    config.UsePublishFilter(typeof(TenantPublishFilter<>), context, x => x.Include<Email>());
                    config.UseConsumeFilter(typeof(TenantConsumeFilter<>), context);
                    config.UsePublishFilter<TenantPublishEmailFilter>(context);
                    //config.ReceiveEndpoint("order-created", 
                    //    e => 
                    //    { 
                    //        e.ConfigureConsumer<OrderCreatedConsumer>(context);
                    //    });
                    config.ConfigureEndpoints(context);
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    serviceScope.ServiceProvider.GetService<OrderContext>().Database.EnsureCreated();
                }
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
