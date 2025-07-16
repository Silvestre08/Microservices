using Contracts.Events;
using MassTransit;
using Orders.Domain.Entities;
using Orders.Service;
using System;
using System.Threading.Tasks;

namespace AdminNotification.Worker
{
    public class OrderCreatedNotification : IConsumer<OrderCreated>
    {
        private readonly IOrderService _orderService;

        public OrderCreatedNotification(IOrderService orderService)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        public async Task Consume(ConsumeContext<OrderCreated> context)
        {
            await context.Publish(new InvoiceNeeded
            { 
                Id = context.Message.Id,
                OrderId = context.Message.OrderId,
                TotalAmount = context.Message.TotalAmount,
                Vat = context.Message.TotalAmount * 1.19m,
            });

            var existingOrder = await _orderService.GetOrderAsync(context.Message.OrderId);

            if(existingOrder != null)
            {
                existingOrder.Status = OrderStatus.Created;
                await _orderService.UpdateOrderAsync(existingOrder);
            }

            await Task.Delay(1000);
            Console.WriteLine(context.Message);
        }
    }
}
