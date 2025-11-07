using AutoMapper;
using Microsoft.Extensions.Logging;
using solidhardware.storeCore.Domain.Entites;
using solidhardware.storeCore.Domain.IRepositoryContract;
using solidhardware.storeCore.DTO.CartDTO;
using solidhardware.storeCore.Helper;
using solidhardware.storeCore.IUnitofWork;
using solidhardware.storeCore.ServiceContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace solidhardware.storeCore.Service
{
    public class CartService : ICartService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<CartService> _logger; 
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICartReposito _cartRepository;

        // Constants
        private const string CART_FULL_INCLUDES = "CartItems.Product.Brand,CartItems.Product.Images";

        public CartService(
            IMapper mapper,
            ILogger<CartService> logger,
            IUnitOfWork unitOfWork,
            ICartReposito cartRepository) 
        {
            _mapper = mapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _cartRepository = cartRepository;
        }

       
        public async Task<CartResponse> AddItemAsync(Guid userId, Guid productId, int quantity = 1)
        {
            _logger.LogInformation(
                "Adding product {ProductId} (qty: {Quantity}) to cart for user {UserId}",
                productId, quantity, userId);

            // Validation
            ValidateGuid(userId, nameof(userId));
            ValidateGuid(productId, nameof(productId));

            if (quantity <= 0)
            {
                _logger.LogError("Invalid quantity: {Quantity}", quantity);
                throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));
            }

            // Get or create cart
            var cart = await _unitOfWork.Repository<Cart>()
                .GetByAsync(
                    c => c.UserId == userId,
                    includeProperties: CART_FULL_INCLUDES);

            if (cart == null)
            {
                _logger.LogInformation("Cart not found for user {UserId}, creating new one", userId);

                cart = new Cart
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                   
                    CartItems = new List<Cartitem>() // ✅ Fixed
                };

                await _unitOfWork.Repository<Cart>().CreateAsync(cart);
                await _unitOfWork.CompleteAsync();
            }

            // Verify product exists
            var product = await _unitOfWork.Repository<Product>()
                .GetByAsync(p => p.Id == productId);

            if (product == null)
            {
                _logger.LogWarning("Product {ProductId} not found", productId);
                throw new KeyNotFoundException("Product not found");
            }

            // Check if product already in cart
            var existingItem = cart.CartItems?.FirstOrDefault(ci => ci.ProductId == productId);

            if (existingItem != null)
            {
                // Update quantity
                _logger.LogInformation(
                    "Product {ProductId} already in cart, updating quantity from {OldQty} to {NewQty}",
                    productId, existingItem.Quantity, existingItem.Quantity + quantity);

                existingItem.Quantity += quantity;

            }
            else
            {
                // Add new item
                var newCartItem = new Cartitem // ✅ Fixed
                {
                    Id = Guid.NewGuid(),
                    ProductId = productId,
                    CartId = cart.Id,
                    Quantity = quantity,
                
                };

                await _unitOfWork.Repository<Cartitem>().CreateAsync(newCartItem);

                _logger.LogInformation(
                    "Added new product {ProductId} to cart for user {UserId}",
                    productId, userId);
            }

            // Update cart timestamp
         
           await   _cartRepository.UpdateAsync(cart); // ✅ Fixed

            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Cart updated successfully for user {UserId}", userId);

            // Get updated cart with all details
            var updatedCart = await _unitOfWork.Repository<Cart>()
                .GetByAsync(
                    c => c.Id == cart.Id,
                    includeProperties: CART_FULL_INCLUDES);

            return _mapper.Map<CartResponse>(updatedCart);
        }

        /// <summary>
        /// Gets or creates cart for user
        /// </summary>
        public async Task<CartResponse> GetOrCreateCartAsync(Guid userId)
        {
            _logger.LogInformation("Getting or creating cart for user {UserId}", userId);

            ValidateGuid(userId, nameof(userId));

            var cart = await _unitOfWork.Repository<Cart>()
                .GetByAsync(
                    c => c.UserId == userId,
                    includeProperties: CART_FULL_INCLUDES);

            if (cart != null)
            {
                _logger.LogInformation("Cart found for user {UserId}", userId);
                return _mapper.Map<CartResponse>(cart);
            }

            // Create new cart
            cart = new Cart
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                
                CartItems = new List<Cartitem>()
            };

            await _unitOfWork.Repository<Cart>().CreateAsync(cart);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Cart created for user {UserId}", userId);

            return _mapper.Map<CartResponse>(cart);
        }

        /// <summary>
        /// Gets cart for specific user
        /// </summary>
        public async Task<CartResponse> GetCartByUserIdAsync(Guid userId)
        {
            _logger.LogInformation("Getting cart for user {UserId}", userId);

            ValidateGuid(userId, nameof(userId));

            var cart = await _unitOfWork.Repository<Cart>()
                .GetByAsync(
                    c => c.UserId == userId,
                    includeProperties: CART_FULL_INCLUDES);

            if (cart == null)
            {
                _logger.LogWarning("Cart not found for user {UserId}", userId);
                throw new KeyNotFoundException("Cart not found for the user");
            }

            return _mapper.Map<CartResponse>(cart);
        }

        /// <summary>
        /// Updates quantity of a cart item
        /// </summary>
        public async Task<CartResponse> UpdateItemQuantityAsync(
            Guid userId,
            Guid cartItemId,
            int quantity)
        {
            _logger.LogInformation(
                "Updating cart item {CartItemId} quantity to {Quantity} for user {UserId}",
                cartItemId, quantity, userId);

            ValidateGuid(userId, nameof(userId));
            ValidateGuid(cartItemId, nameof(cartItemId));

            if (quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));
            }

            var cart = await _unitOfWork.Repository<Cart>()
                .GetByAsync(
                    c => c.UserId == userId,
                    includeProperties: "CartItems");

            if (cart == null)
            {
                _logger.LogWarning("Cart not found for user {UserId}", userId);
                throw new KeyNotFoundException("Cart not found for the user");
            }

            var cartItem = cart.CartItems?.FirstOrDefault(ci => ci.Id == cartItemId);

            if (cartItem == null)
            {
                _logger.LogWarning("Cart item {CartItemId} not found", cartItemId);
                throw new KeyNotFoundException("Cart item not found");
            }

            cartItem.Quantity = quantity;
          

     

            await  _cartRepository.UpdateAsync(cart);

            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Cart item {CartItemId} quantity updated successfully", cartItemId);

            // Get updated cart
            var updatedCart = await _unitOfWork.Repository<Cart>()
                .GetByAsync(
                    c => c.Id == cart.Id,
                    includeProperties: CART_FULL_INCLUDES);

            return _mapper.Map<CartResponse>(updatedCart);
        }

        /// <summary>
        /// Removes an item from cart
        /// </summary>
        public async Task<bool> RemoveItemAsync(Guid userId, Guid cartItemId)
        {
            _logger.LogInformation(
                "Removing cart item {CartItemId} for user {UserId}",
                cartItemId, userId);

            ValidateGuid(userId, nameof(userId));
            ValidateGuid(cartItemId, nameof(cartItemId));

            var cart = await _unitOfWork.Repository<Cart>()
                .GetByAsync(
                    c => c.UserId == userId,
                    includeProperties: "CartItems");

            if (cart == null)
            {
                _logger.LogWarning("Cart not found for user {UserId}", userId);
                throw new KeyNotFoundException("Cart not found for the user");
            }

            var cartItem = cart.CartItems?.FirstOrDefault(ci => ci.Id == cartItemId);

            if (cartItem == null)
            {
                _logger.LogWarning("Cart item {CartItemId} not found", cartItemId);
                throw new KeyNotFoundException("Cart item not found");
            }

            await _unitOfWork.Repository<Cartitem>().DeleteAsync(cartItem);


            await _cartRepository.UpdateAsync(cart);

            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Cart item {CartItemId} removed successfully", cartItemId);

            return true;
        }

        /// <summary>
        /// Clears all items from cart
        /// </summary>
        public async Task<bool> ClearCartAsync(Guid userId)
        {
            _logger.LogInformation("Clearing cart for user {UserId}", userId);

            ValidateGuid(userId, nameof(userId));

            var cart = await _unitOfWork.Repository<Cart>()
                .GetByAsync(
                    c => c.UserId == userId,
                    includeProperties: "CartItems");

            if (cart == null)
            {
                _logger.LogWarning("Cart not found for user {UserId}", userId);
                throw new KeyNotFoundException("Cart not found for the user");
            }

            if (cart.CartItems == null || !cart.CartItems.Any())
            {
                _logger.LogInformation("Cart is already empty for user {UserId}", userId);
                return true;
            }

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var itemCount = cart.CartItems.Count;

                    await _unitOfWork.Repository<Cartitem>()
                        .RemoveRangeAsync(cart.CartItems);


                    await _cartRepository.UpdateAsync(cart);

                    await _unitOfWork.CompleteAsync();
                    await transaction.CommitAsync();

                    _logger.LogInformation(
                        "Cart cleared successfully for user {UserId}, removed {ItemCount} items",
                        userId, itemCount);

                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error clearing cart for user {UserId}", userId);
                    throw;
                }
            }
        }

        /// <summary>
        /// Checks if product exists in user's cart
        /// </summary>
        public async Task<bool> IsProductInCartAsync(Guid userId, Guid productId)
        {
            _logger.LogInformation(
                "Checking if product {ProductId} is in cart for user {UserId}",
                productId, userId);

            ValidateGuid(userId, nameof(userId));
            ValidateGuid(productId, nameof(productId));

            var cart = await _unitOfWork.Repository<Cart>()
                .GetByAsync(
                    c => c.UserId == userId,
                    includeProperties: "CartItems");

            if (cart == null)
            {
                return false;
            }

            var exists = cart.CartItems?.Any(ci => ci.ProductId == productId) ?? false;

            _logger.LogInformation(
                "Product {ProductId} is {Status} in cart for user {UserId}",
                productId, exists ? "present" : "not present", userId);

            return exists;
        }

        /// <summary>
        /// Gets cart item count for user
        /// </summary>
        public async Task<int> GetCartItemCountAsync(Guid userId)
        {
            _logger.LogInformation("Getting cart item count for user {UserId}", userId);

            ValidateGuid(userId, nameof(userId));

            var cart = await _unitOfWork.Repository<Cart>()
                .GetByAsync(
                    c => c.UserId == userId,
                    includeProperties: "CartItems");

            if (cart == null || cart.CartItems == null)
            {
                return 0;
            }

            // Sum all quantities
            var count = cart.CartItems.Sum(ci => ci.Quantity);

            _logger.LogInformation("User {UserId} has {ItemCount} items in cart", userId, count);

            return count;
        }

        /// <summary>
        /// Gets total cart value
        /// </summary>
        public async Task<decimal> GetCartTotalAsync(Guid userId)
        {
            _logger.LogInformation("Getting cart total for user {UserId}", userId);

            ValidateGuid(userId, nameof(userId));

            var cart = await _unitOfWork.Repository<Cart>()
                .GetByAsync(
                    c => c.UserId == userId,
                    includeProperties: CART_FULL_INCLUDES);

            if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
            {
                _logger.LogInformation("Cart is empty for user {UserId}", userId);
                return 0;
            }

            var total = cart.CartItems
                .Where(ci => ci.Product != null)
                .Sum(ci => ci.Quantity * ci.Product.Price);

            _logger.LogInformation("Cart total for user {UserId} is {Total}", userId, total);

            return total;
        }

        private void ValidateGuid(Guid id, string paramName)
        {
            if (id == Guid.Empty)
            {
                _logger.LogError("{ParamName} cannot be empty", paramName);
                throw new ArgumentException($"{paramName} cannot be empty", paramName);
            }
        }

        public Task<CartResponse> AddItemAsync(CartAddRequest cartAddRequest)
        {
            throw new NotImplementedException();
        }
    }
}