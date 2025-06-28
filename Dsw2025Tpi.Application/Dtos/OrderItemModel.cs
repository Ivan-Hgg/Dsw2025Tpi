using System;

namespace Dsw2025Tpi.Application.Dtos
{
    public static class OrderItemModel
    {
        public record OrderItemRequest(
            Guid ProductId,
            int Quantity,
            string? Name,
            string? Description,
            decimal UnitPrice
        );

        public record Response(
            Guid Id,
            Guid ProductId,
            string ProductName,
            string? ProductDescription,
            int Quantity,
            decimal Price,
            decimal Subtotal
        );
    }
}

