using System;

namespace Dsw2025Tpi.Application.Dtos
{
    public static class OrderModel
    {
        public record OrderRequest(
            Guid CustomerId,
            string ShippingAddress,
            string BillingAddress,
            List<OrderItemModel.OrderItemRequest> OrderItems
        );

        public record Response(
            Guid Id,
            Guid CustomerId,
            string ShippingAddress,
            string BillingAddress,
            DateTime Date,
            decimal TotalAmount,
            string Status,
            List<OrderItemModel.Response> OrderItems
        );
    }
}

