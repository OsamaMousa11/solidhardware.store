using solidhardware.storeCore.DTO.WishListDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solidhardware.storeCore.ServiceContract
{
    public interface IWishListService
    {
        Task<WishListResponse> GetOrCreateAsync(Guid userId);
        Task<WishListResponse> AddItemAsync(Guid userId, Guid productId);
        Task<bool> RemoveItemAsync(Guid userId, Guid productId);
        Task<bool> ClearAsync(Guid userId);
        Task<WishListResponse> GetByUserIdAsync(Guid userId);
        Task<bool> IsInWishlistAsync(Guid userId, Guid productId);
    }
}
