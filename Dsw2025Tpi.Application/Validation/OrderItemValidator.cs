using System;
using Dsw2025Tpi.Application.Dtos;

namespace Dsw2025Tpi.Application.Validation
{
    public static class OrderItemValidator
    {
        public static void Validate(OrderItemModel.OrderItemRequest item)
        {
            if (item == null)
                throw new InvalidOperationException("El ítem de la orden no puede ser nulo.");

            if (item.ProductId == Guid.Empty)
                throw new InvalidOperationException("El producto es obligatorio.");

            if (item.Quantity <= 0)
                throw new InvalidOperationException("La cantidad debe ser mayor a cero.");
        }
    }
}
