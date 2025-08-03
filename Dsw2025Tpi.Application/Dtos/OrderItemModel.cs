using System;

namespace Dsw2025Tpi.Application.Dtos
{
    public static class OrderItemModel
    {
        public record OrderItemRequest(
            Guid ProductId,
            int Quantity
        );

        public record Response(
            Guid OrderItemId,
            Guid ProductId,
            string ProductName,
            string? ProductDescription,
            int Quantity,
            decimal Price,
            decimal Subtotal
        );
    }
}

