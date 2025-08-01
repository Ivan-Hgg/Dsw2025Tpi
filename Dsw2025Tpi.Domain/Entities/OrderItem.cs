using System;
using System.ComponentModel;

namespace Dsw2025Tpi.Domain.Entities
{
    public class OrderItem : EntityBase
    {
        public Guid OrderId { get; set; }
        public Order? Order { get; set; }
        public Guid ProductId { get; set; }
        public Product? Product { get; set; }
        public int Quantity { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public OrderItem() { }

        public OrderItem(Guid orderId, Guid productId, int quantity, string? description, decimal price)
        {
            OrderId = orderId;
            ProductId = productId;
            Quantity = quantity;
            Description = description;
            Price = price;
        }

        
    }
}
