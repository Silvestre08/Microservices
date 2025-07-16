
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Events
{
    [EntityName("OrderCReted")]
    public class OrderCreated
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public DateTime CreatedAt { get; set; }

        public Decimal TotalAmount { get; set; }
    }
}
