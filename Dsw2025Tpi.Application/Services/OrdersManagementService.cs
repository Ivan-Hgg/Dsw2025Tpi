using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Validation;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Services
{
    public class OrdersManagementService
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
                var product = await _repository.GetById<Product>(item.ProductId)
                    ?? throw new InvalidOperationException($"Producto no encontrado: {item.ProductId}");

                product.StockQuantity -= item.Quantity;
                await _repository.Update(product);

                var orderItem = new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    Price = product.CurrentUnitPrice
                };
                orderItems.Add(orderItem);
                totalAmount += product.CurrentUnitPrice * item.Quantity;
            }

            var order = new Order
            {
                Id = Guid.NewGuid(),
                CustomerId = request.CustomerId,
                ShippingAddress = request.ShippingAddress,
                BillingAddress = request.BillingAddress,
                Date = DateTime.UtcNow,
                TotalAmount = totalAmount,
                Status = OrderStatus.PENDING,
                OrderItems = orderItems
            };

            await _repository.Add(order);

            var responseItems = orderItems.Select(oi => new OrderItemModel.Response(
                oi.Id,
                oi.ProductId,
                oi.Product?.Name ?? "",
                oi.Product?.Description ?? "",
                oi.Quantity,
                oi.Price,
                oi.Price * oi.Quantity
            )).ToList();

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

    }
}

