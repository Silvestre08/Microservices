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
            endpointConfigurator.UseMessageRetry(r =>
            {
                // Retry the message 5 times with intervals of 10, 20, and 30 seconds
                r.Immediate(5);
                r.Ignore<OrderTooSmallExcdeption>();
            });
        }
    }
}
