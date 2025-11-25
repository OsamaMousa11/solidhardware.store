using solidhardware.storeCore.DTO.CartDTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace solidhardware.storeCore.ServiceContract
{
    public interface ICartService
    {
 
        Task<CartResponse> AddOrUpdateItemAsync(Guid userId, Guid productId, int quantity);


        Task<bool> RemoveItemAsync(Guid userId, Guid productId);


        Task<CartResponse> UpdateItemQuantityAsync(Guid userId, Guid productId, int quantity);

        Task<bool> ClearCartAsync(Guid userId);


        Task<CartResponse> GetCartAsync(Guid userId);


        Task<bool> IsProductInCartAsync(Guid userId, Guid productId);

        Task<int> GetCartItemCountAsync(Guid userId);

        Task<decimal> GetCartSubtotalAsync(Guid userId);
    }
}
