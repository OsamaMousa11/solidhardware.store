using solidhardware.storeCore.DTO.CartDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solidhardware.storeCore.ServiceContract
{
    public interface ICartService
    {
        Task<CartResponse> AddItemAsync(CartAddRequest cartAddRequest);
        Task<CartResponse> AddItemAsync(Guid userId, Guid productId, int quantity = 1);
    }
}
