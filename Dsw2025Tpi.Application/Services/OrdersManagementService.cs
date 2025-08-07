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
using Microsoft.Extensions.Logging;

namespace Dsw2025Tpi.Application.Services
{
    public class OrdersManagementService : IOrdersManagementService
    {
        private readonly IRepository _repository;
        private readonly ILogger<OrdersManagementService> _logger;

        public OrdersManagementService(IRepository repository, ILogger<OrdersManagementService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<OrderModel.Response> CreateOrderAsync(OrderModel.OrderRequest request)
        {
            if (request == null || request.OrderItems == null || request.OrderItems.Count == 0)
                throw new BadRequestException("Datos de la orden inválidos o incompletos.");

            OrderValidator.Validate(request);
            
            if(await _repository.GetById<Customer>(request.CustomerId) is null) 
                throw new BadRequestException($"El CustomerId ingresado no pertenece a ningun cliente: {request.CustomerId}");

            var orderItems = new List<OrderItem>();
            decimal totalAmount = 0;

            foreach (var item in request.OrderItems)
            {
                OrderItemValidator.Validate(item);
            }

            // Descuenta stock y arma los ítems
            foreach (var item in request.OrderItems)
            {
                // Incluye el producto para la respuesta
                var product = await _repository.GetById<Product>(item.ProductId)
                    ?? throw new BadRequestException($"Producto no encontrado: {item.ProductId}");

                if (product.StockQuantity < item.Quantity)
                {
                    foreach(var orderItemm in orderItems)
                    {
                        var productBad = await _repository.GetById<Product>(orderItemm.ProductId);
                        productBad.StockQuantity += orderItemm.Quantity;
                        await _repository.Update(productBad);
                    }
                    throw new BadRequestException($"Stock insuficiente para el producto: {product.Name}");
                }

                if (!product.IsActive)
                    throw new BadRequestException($"El producto {product.Name} está desactivado y no puede ser comprado.");

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
            _logger.LogInformation($"Se creó una nueva orden de ID: {order.Id}");

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
            _logger.LogInformation("Obteniendo todas las órdenes por filtro");
            var pageNumber = filter.PageNumber ?? 1;
            var pageSize = filter.PageSize ?? 10;
            var skip = (pageNumber - 1) * pageSize;

            if (filter.CustomerId is not null)
            {
                var custId = filter.CustomerId.Value;
                if (await _repository.GetById<Customer>(custId) is null)
                    throw new EntityNotFoundException($"El CustomerId ingresado no pertenece a ningun cliente: {filter.CustomerId}");
            }

            if (filter.CustomerId is null && !filter.Status.HasValue)
            {
                var orders = await _repository.GetAll<Order>(nameof(Order.OrderItems), nameof(Order.OrderItems) + "." + nameof(OrderItem.Product));
                orders = orders?.Skip(skip).Take(pageSize);
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
                orders = orders?.Skip(skip).Take(pageSize);
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
                orders = orders?.Skip(skip).Take(pageSize);
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
                orders = orders?.Skip(skip).Take(pageSize);
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
            throw new Exception();
        }

        public async Task<OrderModel.ResponseStatus?> UpdateOrderStatusAsync(Guid OrderId, OrderModel.OrderRequestStatus status)
        {
            if (OrderId == Guid.Empty)
                throw new BadRequestException("El OrderId no puede ser nulo o vacío.");
            var order = await _repository.GetById<Order>(OrderId, nameof(Order.OrderItems), // incluye los ítems de la orden
                nameof(Order.OrderItems) + "." + nameof(OrderItem.Product)) 
                ?? throw new EntityNotFoundException($"Orden no encontrada: {OrderId}");

            order.Status = OrderValidator.ValidateNewStatus(status, order.Status);
            await _repository.Update(order);
            _logger.LogInformation($"Se actualizó el estado de la orden {OrderId} a {status.newStatus}");

            if (order.Status == OrderStatus.CANCELLED)
            {
                await UpdateStockOnOrderCancellation(order);
            }

            return new OrderModel.ResponseStatus(
                order.Id,
                order.Status.ToString()
            );
        }

        private async Task UpdateStockOnOrderCancellation(Order order)
        {
            _logger.LogInformation($"Reponiendo stock por cancelación de la orden {order.Id}");
            foreach (var item in order.OrderItems)
            {
                var product = await _repository.GetById<Product>(item.ProductId);
                if (product != null)
                {
                    product.StockQuantity += item.Quantity;
                    await _repository.Update(product);
                }
            }
        }

        public async Task<OrderModel.Response?> GetOrderByIdAsync(Guid id)
        {
            _logger.LogInformation($"Se buscó una orden por ID: {id}");
            if (id==Guid.Empty) throw new BadRequestException("El OrderId no puede vacío.");

            var order = await _repository.GetById<Order>(id, 
                nameof(Order.OrderItems), // incluye los ítems de la orden
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