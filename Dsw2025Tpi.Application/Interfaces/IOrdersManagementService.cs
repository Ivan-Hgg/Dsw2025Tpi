using Dsw2025Tpi.Application.Dtos;

namespace Dsw2025Tpi.Application.Interfaces
{
    public interface IOrdersManagementService
    {
        Task<OrderModel.Response> CreateOrderAsync(OrderModel.OrderRequest request);
        Task<IEnumerable<OrderModel.Response>?> GetAllOrdersAsync(OrderModel.OrderRequestFilter filter);
        Task<OrderModel.ResponseStatus?> UpdateOrderStatusAsync(Guid OrderId, OrderModel.OrderRequestStatus status);
        Task<OrderModel.Response?> GetOrderByIdAsync(Guid OrderId);
    }
}
