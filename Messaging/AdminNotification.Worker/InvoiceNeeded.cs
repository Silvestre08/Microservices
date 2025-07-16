using System;

namespace AdminNotification.Worker
{
    public class InvoiceNeeded
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Vat
        {
            get; set;
        }
    }
}