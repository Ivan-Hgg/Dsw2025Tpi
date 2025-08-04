using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Validation;
using Dsw2025Tpi.Application.Interfaces;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dsw2025Tpi.Application.Exceptions;
using System.Formats.Asn1;

namespace Dsw2025Tpi.Application.Services
{
    public class OrdersManagementService : IOrdersManagementService
    {
        private readonly IRepository _repository;

        public OrdersManagementService(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<OrderModel.Response> CreateOrderAsync(OrderModel.OrderRequest request)
        {
            OrderValidator.Validate(request);

            var orderItems = new List<OrderItem>();
            decimal totalAmount = 0;

            // Verifica stock y existencia de productos
            foreach (var item in request.OrderItems)
            {
                OrderItemValidator.Validate(item);
            }

            // Descuenta stock y arma los ítems
            foreach (var item in request.OrderItems)
            {
                // Incluye el producto para la respuesta
                var product = await _repository.GetById<Product>(item.ProductId)
                    ?? throw new InvalidOperationException($"Producto no encontrado: {item.ProductId}");

                if (product.StockQuantity < item.Quantity)
                    throw new InvalidOperationException($"Stock insuficiente para el producto: {product.Name}");

                if (!product.IsActive)
                    throw new InvalidOperationException($"El producto {product.Name} está desactivado y no puede ser comprado.");

                product.StockQuantity -= item.Quantity;
                await _repository.Update(product);

                var orderItem = new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    Price = product.CurrentUnitPrice,
                    Description = product.Description,
                    Product = product
                };
                orderItems.Add(orderItem);
                totalAmount += product.CurrentUnitPrice * item.Quantity;
            }

            var order = new Order
            {
                CustomerId = request.CustomerId,
                ShippingAddress = request.ShippingAddress,
                BillingAddress = request.BillingAddress,
                Date = DateTime.UtcNow,
                TotalAmount = totalAmount,
                Status = OrderStatus.PENDING,
                OrderItems = orderItems
            };

            await _repository.Add(order);

            // Los productos ya están asignados en los OrderItem
            var responseItems = orderItems.Select(oi => new OrderItemModel.Response(
                oi.Id,
                oi.ProductId,
                oi.Product?.Name ?? "",
                oi.Product?.Description ?? "",
                oi.Quantity,
                oi.Price,
                oi.Price * oi.Quantity
            ));

            return new OrderModel.Response(
                order.Id,
                order.CustomerId ?? Guid.Empty,
                order.ShippingAddress,
                order.BillingAddress,
                order.Date,
                order.TotalAmount,
                order.Status.ToString(),
                responseItems
            );
        }
        public async Task<IEnumerable<OrderModel.Response>?> GetAllOrdersAsync(OrderModel.OrderRequestFilter filter)
        {
            if (filter.CustomerId is null && !filter.Status.HasValue)
            {
                var orders = await _repository.GetAll<Order>(nameof(Order.OrderItems), nameof(Order.OrderItems) + "." + nameof(OrderItem.Product));
                return orders?.Select(o => new OrderModel.Response(
                    o.Id,
                    o.CustomerId ?? Guid.Empty,
                    o.ShippingAddress,
                    o.BillingAddress,
                    o.Date,
                    o.TotalAmount,
                    o.Status.ToString(),
                    o.OrderItems.Select(oi => new OrderItemModel.Response(
                        oi.Id,
                        oi.ProductId,
                        oi.Product?.Name ?? "",
                        oi.Product?.Description ?? "",
                        oi.Quantity,
                        oi.Price,
                        oi.Price * oi.Quantity
                    ))
                ));
            }
            if (filter.CustomerId is not null && !filter.Status.HasValue)
            {
                var orders = await _repository.GetFiltered<Order>(o => o.CustomerId == filter.CustomerId,
                    nameof(Order.OrderItems), nameof(Order.OrderItems) + "." + nameof(OrderItem.Product));
                return orders?.Select(o => new OrderModel.Response(
                    o.Id,
                    o.CustomerId ?? Guid.Empty,
                    o.ShippingAddress,
                    o.BillingAddress,
                    o.Date,
                    o.TotalAmount,
                    o.Status.ToString(),
                    o.OrderItems.Select(oi => new OrderItemModel.Response(
                        oi.Id,
                        oi.ProductId,
                        oi.Product?.Name ?? "",
                        oi.Product?.Description ?? "",
                        oi.Quantity,
                        oi.Price,
                        oi.Price * oi.Quantity
                    ))
                ));
            }
            if (filter.CustomerId is null && filter.Status.HasValue)
            {
                var orders = await _repository.GetFiltered<Order>(o => o.Status == filter.Status,
                    nameof(Order.OrderItems), nameof(Order.OrderItems) + "." + nameof(OrderItem.Product));
                return orders?.Select(o => new OrderModel.Response(
                    o.Id,
                    o.CustomerId ?? Guid.Empty,
                    o.ShippingAddress,
                    o.BillingAddress,
                    o.Date,
                    o.TotalAmount,
                    o.Status.ToString(),
                    o.OrderItems.Select(oi => new OrderItemModel.Response(
                        oi.Id,
                        oi.ProductId,
                        oi.Product?.Name ?? "",
                        oi.Product?.Description ?? "",
                        oi.Quantity,
                        oi.Price,
                        oi.Price * oi.Quantity
                    ))
                ));
            }
            if (filter.CustomerId is not null && filter.Status.HasValue)
            {
                var orders = await _repository.GetFiltered<Order>(o => o.CustomerId == filter.CustomerId && o.Status == filter.Status,
                nameof(Order.OrderItems), // incluye los ítems de la orden
                nameof(Order.OrderItems) + "." + nameof(OrderItem.Product) // incluye el producto dentro de los ítems
                );

                return orders?.Select(o => new OrderModel.Response(
                    o.Id,
                    o.CustomerId ?? Guid.Empty,
                    o.ShippingAddress,
                    o.BillingAddress,
                    o.Date,
                    o.TotalAmount,
                    o.Status.ToString(),
                    o.OrderItems.Select(oi => new OrderItemModel.Response(
                        oi.Id,
                        oi.ProductId,
                        oi.Product?.Name ?? "",
                        oi.Product?.Description ?? "",
                        oi.Quantity,
                        oi.Price,
                        oi.Price * oi.Quantity
                    ))
                ));

            }
            throw new Exception("Fallo en el filtro.");
        }
        public async Task<OrderModel.ResponseStatus?> UpdateOrderStatusAsync(Guid OrderId, OrderModel.OrderRequestStatus status)
        {
            if (string.IsNullOrWhiteSpace(status.newStatus)) throw new ArgumentException("El nuevo estado no puede ser nulo o vacío.");

            var order = await _repository.GetById<Order>(OrderId, nameof(Order.OrderItems), // incluye los ítems de la orden
                nameof(Order.OrderItems) + "." + nameof(OrderItem.Product)) // incluye el producto dentro de los ítems
                ?? throw new EntityNotFoundException($"Orden no encontrada: {OrderId}");
            order.Status = Enum.TryParse<OrderStatus>(status.newStatus, true, out var newStatus)
                ? newStatus
                : throw new ArgumentException($"Estado de orden inválido: {status.newStatus}");
            await _repository.Update(order);
            return new OrderModel.ResponseStatus(
                order.Id,
                order.Status.ToString()
            );
        }

        public async Task<OrderModel.Response?> GetOrderByIdAsync(Guid id)
        {
            var order = await _repository.GetById<Order>(id, nameof(Order.OrderItems), // incluye los ítems de la orden
                nameof(Order.OrderItems) + "." + nameof(OrderItem.Product)) // incluye el producto dentro de los ítems
                ?? throw new EntityNotFoundException($"Orden no encontrada: {id}");
            return new OrderModel.Response(
                order.Id,
                order.CustomerId ?? Guid.Empty,
                order.ShippingAddress,
                order.BillingAddress,
                order.Date,
                order.TotalAmount,
                order.Status.ToString(),
                order.OrderItems.Select(oi => new OrderItemModel.Response(
                    oi.Id,
                    oi.ProductId,
                    oi.Product?.Name ?? "",
                    oi.Product?.Description ?? "",
                    oi.Quantity,
                    oi.Price,
                    oi.Price * oi.Quantity
                ))
            );


        }
    }
}

