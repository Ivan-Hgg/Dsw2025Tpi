using Dsw2025Tpi.Application.Dtos;

namespace Dsw2025Tpi.Application.Interfaces
{
    public interface IOrdersManagementService
    {
        Task<OrderModel.Response> CreateOrderAsync(OrderModel.OrderRequest request);
        Task<IEnumerable<OrderModel.Response>?> GetAllOrdersAsync(OrderModel.OrderRequestFilter filter);
        Task<OrderModel.Response?> UpdateOrderStatusAsync(Guid OrderId, OrderModel.OrderRequestStatus status);
    }
}
