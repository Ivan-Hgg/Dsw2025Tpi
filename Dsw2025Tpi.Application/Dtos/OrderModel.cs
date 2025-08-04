using Dsw2025Tpi.Domain.Entities;
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
        public record OrderRequestFilter(
            Guid? CustomerId,
            OrderStatus? Status,
            int? PageNumber,
            int? PageSize
        );

        public record OrderRequestStatus(
            string newStatus
        );

        public record Response(
            Guid OrderId,
            Guid CustomerId,
            string ShippingAddress,
            string BillingAddress,
            DateTime Date,
            decimal TotalAmount,
            string Status,
            IEnumerable<OrderItemModel.Response> OrderItems
        ); 
        public record ResponseStatus(
            Guid OrderId,
            string Status
        );
    }
}

