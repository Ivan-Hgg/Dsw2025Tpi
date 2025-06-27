using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Validation
{
    using Dsw2025Tpi.Application.Dtos;

    public static class OrderValidator
    {
        public static void Validate(OrderModel.OrderRequest request)
        {
            if (request == null)
                throw new InvalidOperationException("La orden no puede ser nula.");

            if (request.CustomerId == Guid.Empty)
                throw new InvalidOperationException("El cliente es obligatorio.");

            if (string.IsNullOrWhiteSpace(request.ShippingAddress))
                throw new InvalidOperationException("La dirección de envío es obligatoria.");

            if (string.IsNullOrWhiteSpace(request.BillingAddress))
                throw new InvalidOperationException("La dirección de facturación es obligatoria.");

            if (request.OrderItems == null || !request.OrderItems.Any())
                throw new InvalidOperationException("Debe haber al menos un ítem en la orden.");

            foreach (var item in request.OrderItems)
            {
                OrderItemValidator.Validate(item);
            }
        }
    }
}

