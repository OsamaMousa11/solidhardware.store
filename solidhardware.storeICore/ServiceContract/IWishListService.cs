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
        Task<WishListResponse> GetByIdAsync(Guid wishlistId); 
        Task<WishListResponse> GetWishlistByUserIdAsync(Guid userId); 

        Task<WishListResponse> GetOrCreateWishlistAsync(Guid userId);
        Task<bool> ClearUserWishlistAsync(Guid userId);

        Task<WishListResponse> AddWishlistAsync(WishListAddRequest request);
        Task<WishListResponse> AddItemAsync(Guid userId, Guid productId);
        Task<bool> RemoveItemAsync(Guid userId, Guid wishlistItemId);
        Task<bool> IsProductInWishlistAsync(Guid userId, Guid productId);

        Task<bool> MoveItemToCartAsync(Guid userId, Guid wishlistItemId, int quantity);
        Task<bool> MoveAllToCartAsync(Guid userId);
    }
}
