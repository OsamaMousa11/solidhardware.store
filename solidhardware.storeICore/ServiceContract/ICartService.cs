using solidhardware.storeCore.DTO.CartDTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace solidhardware.storeCore.ServiceContract
{
    public interface ICartService
    {
        /// <summary>
        /// Add a single product to a user's cart (creates cart if not exists)
        /// </summary>
        Task<CartResponse> AddItemAsync(Guid userId, Guid productId, int quantity = 1);

        /// <summary>
        /// Create a new cart (optionally with multiple items)
        /// </summary>
        Task<CartResponse> AddCartAsync(CartAddRequest request);

        /// <summary>
        /// Update quantity of a specific product in the cart
        /// </summary>
        Task<CartResponse> UpdateItemQuantityAsync(CartUpdateRequest request);

        /// <summary>
        /// Remove a specific item from the cart
        /// </summary>
        Task<bool> RemoveItemAsync(Guid userId, Guid cartItemId);

        /// <summary>
        /// Remove all items from the user's cart
        /// </summary>
        Task<bool> ClearCartAsync(Guid userId);

        /// <summary>
        /// Get or create cart for a specific user
        /// </summary>
        Task<CartResponse> GetOrCreateCartAsync(Guid userId);

        /// <summary>
        /// Get a user's cart (with items, products, and details)
        /// </summary>
        Task<CartResponse> GetCartByUserIdAsync(Guid userId);

        /// <summary>
        /// Get a cart by its ID
        /// </summary>
        Task<CartResponse> GetCartByIdAsync(Guid cartId);

        /// <summary>
        /// Get all items in a user's cart
        /// </summary>
        Task<List<CartItemResponse>> GetCartItemsAsync(Guid userId);

        /// <summary>
        /// Get the total number of items in the user's cart
        /// </summary>
        Task<int> GetCartItemCountAsync(Guid userId);

        /// <summary>
        /// Get the total cart value (sum of item prices * quantity)
        /// </summary>
        Task<decimal> GetCartTotalAsync(Guid userId);

        /// <summary>
        /// Check if a specific product exists in the user's cart
        /// </summary>
        Task<bool> IsProductInCartAsync(Guid userId, Guid productId);
    }
}
