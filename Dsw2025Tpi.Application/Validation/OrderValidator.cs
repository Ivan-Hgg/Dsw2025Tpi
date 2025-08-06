using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Domain.Entities;
using System;

namespace Dsw2025Tpi.Application.Validation
{
    public static class OrderValidator
    {
        public static void Validate(OrderModel.OrderRequest request)
        {
            if (request == null)
                throw new BadRequestException("La orden no puede ser nula.");

            if (request.CustomerId == Guid.Empty)
                throw new BadRequestException("El cliente es obligatorio.");

            if (string.IsNullOrWhiteSpace(request.ShippingAddress) || request.ShippingAddress.Length > 256)
                throw new BadRequestException("La dirección de envío es obligatoria y no puede superar los 256 caracteres.");

            if (string.IsNullOrWhiteSpace(request.BillingAddress) || request.BillingAddress.Length > 256)
                throw new BadRequestException("La dirección de facturación es obligatoria y no puede superar los 256 caracteres.");

            if (request.OrderItems == null || request.OrderItems.Count == 0)
                throw new BadRequestException("Debe incluir al menos un ítem en la orden.");
        }

        public static OrderStatus ValidateNewStatus(OrderModel.OrderRequestStatus newStatus, OrderStatus oldStatus)
        {
            if (newStatus == null)
                throw new BadRequestException("El nuevo estado de la orden no puede ser nulo.");
            if (string.IsNullOrWhiteSpace(newStatus.newStatus))
                throw new BadRequestException("El nuevo estado no puede ser nulo o vacío..");
            
            var newStatusEnum= Enum.TryParse<OrderStatus>(newStatus.newStatus, true, out var parsedStatus) ? 
                parsedStatus : throw new BadRequestException($"Estado de orden inválido: {newStatus.newStatus}"); 

            if (oldStatus==newStatusEnum) 
                throw new InvalidStatusTransitionException("El nuevo estado de la orden no puede ser el mismo que el actual.");

            switch (oldStatus)
            {
                case OrderStatus.CANCELLED: throw new InvalidStatusTransitionException("No se puede cambiar el estado de una orden que ya ha sido cancelada.");
                case OrderStatus.DELIVERED: throw new InvalidStatusTransitionException("No se puede cambiar el estado de una orden que ya ha sido entregada.");
                case OrderStatus.SHIPPED when newStatusEnum != OrderStatus.DELIVERED:
                    throw new InvalidStatusTransitionException("Una orden enviada solo puede ser entregada.");
                case OrderStatus.PROCESSING when newStatusEnum != OrderStatus.SHIPPED && newStatusEnum != OrderStatus.CANCELLED:
                    throw new InvalidStatusTransitionException("Una orden en proceso solo puede ser enviada o cancelada.");
                case OrderStatus.PENDING when newStatusEnum != OrderStatus.PROCESSING && newStatusEnum != OrderStatus.CANCELLED:
                    throw new InvalidStatusTransitionException("Una orden pendiente solo puede ser procesada o cancelada.");
                default: break;
            }
            return newStatusEnum;


        }

    }
}
