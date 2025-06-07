using Contracts.Events;
using MassTransit;

namespace OrdersApi.Consumer
{
    public class OrderCreateFaultConsumer : IConsumer<Fault<OrderCreated>>
    {
        public Task Consume(ConsumeContext<Fault<OrderCreated>> context)
        {
            Console.WriteLine($"This is a order created fault. The message faulted {context.Message.Message.OrderId}");
            return Task.CompletedTask;
        }
    }
}
