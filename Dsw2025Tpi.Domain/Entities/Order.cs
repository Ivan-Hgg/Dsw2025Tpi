using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Domain.Entities
{
    public class Order : EntityBase
    {
        public OrderStatus Status { get; set; }
        public DateTime Date { get; set; }
        public string ShippingAddress { get; set; } = string.Empty;
        public string BillingAddress { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string? Notes { get; set; }
        public Guid? CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
       public Order() 
        {
            OrderItems = new List<OrderItem>();
            Date = DateTime.UtcNow;
            Status = OrderStatus.PENDING;
        }

        public Order(
            DateTime date,
            string shippingAddress,
            string billingAddress,
            decimal totalAmount,
            string notes)
        {
            Date = date;
            ShippingAddress = shippingAddress;
            BillingAddress = billingAddress;
            TotalAmount = totalAmount;
            Notes= notes?? string.Empty;
            Status = OrderStatus.PENDING;
            OrderItems = new List<OrderItem>();
        }

        
    }
}
