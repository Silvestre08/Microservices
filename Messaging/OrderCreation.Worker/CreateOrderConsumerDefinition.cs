using Contracts.Exceptions;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderCreation.Worker
{
    public class CreateOrderConsumerDefinition : ConsumerDefinition<CreateOrderConsumer>
    {
        public CreateOrderConsumerDefinition()
        {
            // Set the endpoint name to "create-order-command"
            EndpointName = "create-order-command";
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<CreateOrderConsumer> consumerConfigurator)
        {
            consumerConfigurator.UseDelayedRedelivery(r => 
            {
                r.Intervals(TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(60));
            });

            consumerConfigurator.UseMessageRetry(r =>
            {
                // Retry the message 5 times with intervals of 10, 20, and 30 seconds
                r.Immediate(5);
                r.Ignore<OrderTooSmallExcdeption>();
                r.Handle<HandleAllExceptions>();
            });
        }
    }
}
