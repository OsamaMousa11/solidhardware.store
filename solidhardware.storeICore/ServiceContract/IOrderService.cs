using solidhardware.storeCore.DTO.OrderDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solidhardware.storeCore.ServiceContract
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderResponse>> GetOrdersByUserIdAsync(Guid userId);
        Task<OrderResponse> CreateOrderAsync(OrderAddRequest request);

        Task<OrderResponse?> GetOrderByIdAsync(Guid orderId);

        Task<List<OrderResponse>> GetAllOrdersAsync();

        Task<OrderResponse> UpdateOrderAsync(OrderUpdateRequest request);

        Task<bool> DeleteOrderAsync(Guid orderId);
    }

}
