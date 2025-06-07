using Contracts.Events;
using MassTransit;

namespace OrdersApi.Consumer
{
    public class OrderCreatedConsumer : IConsumer<OrderCreated>
    {
        public async Task Consume(ConsumeContext<OrderCreated> context)
        {
            await Task.Delay(1000);
            throw new ArgumentNullException();
           Console.WriteLine(context.Message); 
        }
    }
}
