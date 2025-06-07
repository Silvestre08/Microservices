using MassTransit;

namespace OrdersApi.Consumer
{
    public class AllFaultsConsumer : IConsumer<Fault>
    {
        public Task Consume(ConsumeContext<Fault> context)
        {
            Console.WriteLine($"This is a fault message. The message faulted {context.Message.FaultedMessageId}");
            return Task.CompletedTask;
        }
    }
}
