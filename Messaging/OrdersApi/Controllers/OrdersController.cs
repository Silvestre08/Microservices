﻿using AutoMapper;
using Contracts.Events;
using Contracts.Models;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Orders.Domain.Entities;
using Orders.Service;
using OrdersApi.Service.Clients;

namespace OrdersApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IProductStockServiceClient productStockServiceClient;
        private readonly IMapper mapper;
        private readonly IPublishEndpoint _publishEndpoint;

        public OrdersController(IOrderService orderService,
            IProductStockServiceClient productStockServiceClient,
            IMapper mapper,
            IPublishEndpoint publishEndpoint
            )
        {
            _orderService = orderService;
            this.productStockServiceClient = productStockServiceClient;
            this.mapper = mapper;
            _publishEndpoint = publishEndpoint;
        }


        // POST: api/Orders
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(OrderModel model)
        {
            //verify stock
            //var stocks = await productStockServiceClient.GetStock(
            //    model.OrderItems.Select(p => p.ProductId).ToList());

            var orderToAdd = mapper.Map<Order>(model);
            var createdOrder = await _orderService.AddOrderAsync(orderToAdd);
            var publishOrder = _publishEndpoint.Publish(
                new OrderCreated
                { CreatedAt = createdOrder.OrderDate,
                    Id = createdOrder.Id, OrderId = createdOrder.OrderId,
                    TotalAmount = createdOrder.OrderItems.Sum(i => i.Quantity * i.Price)
                }, 
                context => 
                { 
                    context.Headers.Set("my-custom-header", "value");
                });
            return CreatedAtAction("GetOrder", new { id = createdOrder.Id }, createdOrder);
        }


        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _orderService.GetOrderAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        // PUT: api/Orders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order order)
        {
            if (id != order.Id)
            {
                return BadRequest();
            }

            try
            {
                await _orderService.UpdateOrderAsync(order);
            }
            catch
            {
                if (!await _orderService.OrderExistsAsync(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            var orders = await _orderService.GetOrdersAsync();
            return Ok(orders);
        }



        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _orderService.GetOrderAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            await _orderService.DeleteOrderAsync(id);
            return NoContent();
        }
    }
}
