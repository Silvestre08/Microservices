using AutoMapper;
using Contracts.Events;
using Contracts.Exceptions;
using Contracts.Models;
using MassTransit;
using MassTransit.Transports;
using Orders.Domain.Entities;
using Orders.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderCreation.Worker
{
    public class CreateOrderConsumer : IConsumer<OrderModel>
    {
        private readonly IMapper _mapper;
        private readonly IOrderService _orderService;

        public CreateOrderConsumer(IMapper mapper, IOrderService orderService)
        {    
            _mapper = mapper;
            _orderService = orderService;
        }

        public async Task Consume(ConsumeContext<OrderModel> context)
        {
            Console.WriteLine($"I got a command to create an order: {context.Message}");
            var orderToAdd = _mapper.Map<Order>(context.Message);

            Console.WriteLine($"This is the retry attempt from {context.GetRetryAttempt()} from {context.GetRedeliveryCount()}");

            var total = orderToAdd.OrderItems.Sum(i => i.Quantity * i.Price);
            if(total <= 100)
            {
                throw new OrderTooSmallExcdeption();
            }
            var createdOrder = await _orderService.AddOrderAsync(orderToAdd);

            var publishOrder = context.Publish(
               new OrderCreated
               {
                   CreatedAt = createdOrder.OrderDate,
                   Id = createdOrder.Id,
                   TotalAmount = createdOrder.OrderItems.Sum(i => i.Quantity * i.Price)
               },
               context =>
               {
                   //context.TimeToLive = TimeSpan.FromSeconds(30);
                   context.Headers.Set("my-custom-header", "value");
               });
        }
    }
}
