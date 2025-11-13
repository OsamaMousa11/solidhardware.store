using AutoMapper;
using Microsoft.Extensions.Logging;
using solidhardware.storeCore.Domain.Entites;
using solidhardware.storeCore.Domain.IRepositoryContract;
using solidhardware.storeCore.DTO.CartDTO;
using solidhardware.storeCore.Helper;
using solidhardware.storeCore.IUnitofWork;
using solidhardware.storeCore.ServiceContract;

namespace solidhardware.storeCore.Service
{
    public class CartService : ICartService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<CartService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICartRepository _cartRepository;

        private const string CART_FULL_INCLUDES = "CartItems.Product.Brand,CartItems.Product.Images";

        public CartService(
            IMapper mapper,
            ILogger<CartService> logger,
            IUnitOfWork unitOfWork,
            ICartRepository cartRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _cartRepository = cartRepository;
        }

        // ✅ Add Item
        public async Task<CartResponse> AddItemAsync(Guid userId, Guid productId, int quantity = 1)
        {
            _logger.LogInformation("Adding product {ProductId} (qty: {Quantity}) to cart for user {UserId}",
                productId, quantity, userId);

            ValidateGuid(userId, nameof(userId));
            ValidateGuid(productId, nameof(productId));

            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

            var cart = await _unitOfWork.Repository<Cart>()
                .GetByAsync(c => c.UserId == userId, includeProperties: CART_FULL_INCLUDES);

            if (cart == null)
            {
                cart = new Cart
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CartItems = new List<Cartitem>()
                };

                await _unitOfWork.Repository<Cart>().CreateAsync(cart);
                await _unitOfWork.CompleteAsync();
            }

            var product = await _unitOfWork.Repository<Product>()
                .GetByAsync(p => p.Id == productId)
                ?? throw new KeyNotFoundException("Product not found");

            var existingItem = cart.CartItems?.FirstOrDefault(ci => ci.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                _logger.LogInformation("Updated quantity for product {ProductId} to {Quantity}", productId, existingItem.Quantity);
            }
            else
            {
                var newItem = new Cartitem
                {
                    Id = Guid.NewGuid(),
                    ProductId = productId,
                    CartId = cart.Id,
                    Quantity = quantity
                };

                await _unitOfWork.Repository<Cartitem>().CreateAsync(newItem);
            }

            await _cartRepository.UpdateAsync(cart);
            await _unitOfWork.CompleteAsync();

            var updatedCart = await _unitOfWork.Repository<Cart>()
                .GetByAsync(c => c.Id == cart.Id, includeProperties: CART_FULL_INCLUDES);

            return _mapper.Map<CartResponse>(updatedCart);
        }

        // ✅ Add Cart
        public async Task<CartResponse> AddCartAsync(CartAddRequest request)
        {
            _logger.LogInformation("Creating cart for user {UserId}", request?.UserId);

            if (request == null)
                throw new ArgumentNullException(nameof(request));

            ValidationHelper.ValidateModel(request);

            var existingCart = await _unitOfWork.Repository<Cart>()
                .GetByAsync(c => c.UserId == request.UserId, includeProperties: CART_FULL_INCLUDES);

            if (existingCart != null)
                return _mapper.Map<CartResponse>(existingCart);

            using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                var cart = _mapper.Map<Cart>(request);
                cart.Id = Guid.NewGuid();
                cart.UserId = request.UserId;
                cart.CartItems ??= new List<Cartitem>();

                if (request.cartitemAddRequest != null && request.cartitemAddRequest.Any())
                {
                    cart.CartItems = request.cartitemAddRequest.Select(i => new Cartitem
                    {
                        Id = Guid.NewGuid(),
                        CartId = cart.Id,
                        ProductId = i.ProductId,
                        Quantity = i.Quantity
                    }).ToList();
                }
                else
                {
                    // fallback to single item
                    cart.CartItems.Add(new Cartitem
                    {
                        Id = Guid.NewGuid(),
                        CartId = cart.Id,
                        ProductId = request.ProductId,
                        Quantity = request.Quantity
                    });
                }

                await _unitOfWork.Repository<Cart>().CreateAsync(cart);
                await _unitOfWork.CompleteAsync();
                await transaction.CommitAsync();

                var createdCart = await _unitOfWork.Repository<Cart>()
                    .GetByAsync(c => c.Id == cart.Id, includeProperties: CART_FULL_INCLUDES);

                return _mapper.Map<CartResponse>(createdCart);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Failed to create cart for user {UserId}", request.UserId);
                throw;
            }
        }

        // ✅ Update Quantity
        public async Task<CartResponse> UpdateItemQuantityAsync(CartUpdateRequest request)
        {
            _logger.LogInformation("Updating product {ProductId} quantity to {Quantity} for user {UserId}",
                request.ProductId, request.Quantity, request.UserId);

            ValidateGuid(request.UserId, nameof(request.UserId));
            ValidateGuid(request.ProductId, nameof(request.ProductId));

            var cart = await _unitOfWork.Repository<Cart>()
                .GetByAsync(c => c.UserId == request.UserId, includeProperties: "CartItems.Product")
                ?? throw new KeyNotFoundException("Cart not found");

            var item = cart.CartItems?.FirstOrDefault(ci => ci.ProductId == request.ProductId)
                ?? throw new KeyNotFoundException("Item not found in cart");

            item.Quantity = request.Quantity;

            await _cartRepository.UpdateAsync(cart);
            await _unitOfWork.CompleteAsync();

            var updated = await _unitOfWork.Repository<Cart>()
                .GetByAsync(c => c.Id == cart.Id, includeProperties: CART_FULL_INCLUDES);

            return _mapper.Map<CartResponse>(updated);
        }

        // ✅ Remove Item
        public async Task<bool> RemoveItemAsync(Guid userId, Guid cartItemId)
        {
            _logger.LogInformation("Removing cart item {CartItemId} for user {UserId}", cartItemId, userId);

            var cart = await _unitOfWork.Repository<Cart>()
                .GetByAsync(c => c.UserId == userId, includeProperties: "CartItems")
                ?? throw new KeyNotFoundException("Cart not found");

            var item = cart.CartItems?.FirstOrDefault(i => i.Id == cartItemId)
                ?? throw new KeyNotFoundException("Item not found in cart");

            await _unitOfWork.Repository<Cartitem>().DeleteAsync(item);
            await _cartRepository.UpdateAsync(cart);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        // ✅ Get or Create Cart
        public async Task<CartResponse> GetOrCreateCartAsync(Guid userId)
        {
            _logger.LogInformation("Getting or creating cart for user {UserId}", userId);
            ValidateGuid(userId, nameof(userId));

            var cart = await _unitOfWork.Repository<Cart>()
                .GetByAsync(c => c.UserId == userId, includeProperties: CART_FULL_INCLUDES);

            if (cart != null)
                return _mapper.Map<CartResponse>(cart);

            var newCart = new Cart
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CartItems = new List<Cartitem>()
            };

            await _unitOfWork.Repository<Cart>().CreateAsync(newCart);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<CartResponse>(newCart);
        }

        // ✅ Get Cart by UserId
        public async Task<CartResponse> GetCartByUserIdAsync(Guid userId)
        {
            _logger.LogInformation("Fetching cart for user {UserId}", userId);

            var cart = await _unitOfWork.Repository<Cart>()
                .GetByAsync(c => c.UserId == userId, includeProperties: CART_FULL_INCLUDES)
                ?? throw new KeyNotFoundException("Cart not found for user");

            return _mapper.Map<CartResponse>(cart);
        }

        // ✅ Get Cart by Id
        public async Task<CartResponse> GetCartByIdAsync(Guid cartId)
        {
            _logger.LogInformation("Fetching cart with ID {CartId}", cartId);

            var cart = await _unitOfWork.Repository<Cart>()
                .GetByAsync(c => c.Id == cartId, includeProperties: CART_FULL_INCLUDES)
                ?? throw new KeyNotFoundException("Cart not found");

            return _mapper.Map<CartResponse>(cart);
        }

        // ✅ Get All Cart Items
        public async Task<List<CartItemResponse>> GetCartItemsAsync(Guid userId)
        {
            _logger.LogInformation("Fetching cart items for user {UserId}", userId);

            var cart = await _unitOfWork.Repository<Cart>()
                .GetByAsync(c => c.UserId == userId, includeProperties: CART_FULL_INCLUDES)
                ?? throw new KeyNotFoundException("Cart not found");

            if (cart.CartItems == null || !cart.CartItems.Any())
                return new List<CartItemResponse>();

            return _mapper.Map<List<CartItemResponse>>(cart.CartItems);
        }

        // ✅ Clear Cart
        public async Task<bool> ClearCartAsync(Guid userId)
        {
            _logger.LogInformation("Clearing cart for user {UserId}", userId);

            var cart = await _unitOfWork.Repository<Cart>()
                .GetByAsync(c => c.UserId == userId, includeProperties: "CartItems");

            if (cart == null) throw new KeyNotFoundException("Cart not found");
            if (!cart.CartItems.Any()) return true;

            await _unitOfWork.Repository<Cartitem>().RemoveRangeAsync(cart.CartItems);
            await _cartRepository.UpdateAsync(cart);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        // ✅ Total Value
        public async Task<decimal> GetCartTotalAsync(Guid userId)
        {
            var cart = await _unitOfWork.Repository<Cart>()
                .GetByAsync(c => c.UserId == userId, includeProperties: CART_FULL_INCLUDES);

            return cart?.CartItems?.Sum(ci => ci.Quantity * ci.Product.Price) ?? 0;
        }

        // ✅ Item Count
        public async Task<int> GetCartItemCountAsync(Guid userId)
        {
            var cart = await _unitOfWork.Repository<Cart>()
                .GetByAsync(c => c.UserId == userId, includeProperties: "CartItems");

            return cart?.CartItems?.Sum(ci => ci.Quantity) ?? 0;
        }

        // ✅ Check if Product Exists
        public async Task<bool> IsProductInCartAsync(Guid userId, Guid productId)
        {
            var cart = await _unitOfWork.Repository<Cart>()
                .GetByAsync(c => c.UserId == userId, includeProperties: "CartItems");

            return cart?.CartItems?.Any(ci => ci.ProductId == productId) ?? false;
        }

        // ✅ Helper
        private void ValidateGuid(Guid id, string name)
        {
            if (id == Guid.Empty)
                throw new ArgumentException($"{name} cannot be empty", name);
        }
    }
}
