using Contracts.Response;
using MassTransit;
using Orders.Domain.Entities;
using Orders.Service;

namespace OrdersApi.Consumer
{
    public class VerifyOrderConsumer : IConsumer<VerifyOrder>
    {
        private readonly IOrderService _orderService;

        public VerifyOrderConsumer(IOrderService orderService)
        {
            _orderService = orderService;  
        }

        public async Task Consume(ConsumeContext<VerifyOrder> context)
        {
            var existingOrder = await _orderService.GetOrderAsync(context.Message.OrderId);

            if (!context.IsResponseAccepted<Order>())
            {
                throw new ArgumentException("Type not accepted");
            }

            if (existingOrder != null) 
            {
                await context.RespondAsync<OrderResult>(new OrderResult
                {
                    Id = context.Message.OrderId,
                    OrderDate =  existingOrder.OrderDate,
                    Status = existingOrder.Status,
                });

                return;
            }

            await context.RespondAsync<OrderNotFoundResult>(new OrderNotFoundResult
            {
                ErrorResult = "Order not found."
            });
        }
    }
}
