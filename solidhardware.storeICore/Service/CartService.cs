using AutoMapper;
using Microsoft.Extensions.Logging;
using solidhardware.storeCore.Domain.Entites;
using solidhardware.storeCore.DTO.CartDTO;
using solidhardware.storeCore.IUnitofWork;
using solidhardware.storeCore.ServiceContract;

namespace solidhardware.storeCore.Service
{
    public class CartService : ICartService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<CartService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        private const string CART_ITEMS = "CartItems";
        private const string CART_FULL = "CartItems.Product";

        public CartService(
            IMapper mapper,
            ILogger<CartService> logger,
            IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        // --------------------------------------------------
        // GET or CREATE CART
        // --------------------------------------------------
        public async Task<CartResponse> GetCartAsync(Guid userId)
        {
            var cart = await _unitOfWork.Repository<Cart>()
                .GetByAsync(c => c.UserId == userId, includeProperties: CART_FULL);

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

                // reload to include Product
                cart = await _unitOfWork.Repository<Cart>()
                    .GetByAsync(c => c.Id == cart.Id, includeProperties: CART_FULL);
            }

            return _mapper.Map<CartResponse>(cart);
        }

        // --------------------------------------------------
        // ADD OR UPDATE ITEM
        // --------------------------------------------------
        public async Task<CartResponse> AddOrUpdateItemAsync(Guid userId, Guid productId, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than 0");

            var cart = await _unitOfWork.Repository<Cart>()
                .GetByAsync(c => c.UserId == userId, isTracked: true, includeProperties: CART_FULL);

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

            var item = cart.CartItems.FirstOrDefault(i => i.ProductId == productId);

            if (item == null)
            {
                cart.CartItems.Add(new Cartitem
                {
                    Id = Guid.NewGuid(),
                    ProductId = productId,
                    CartId = cart.Id,
                    Quantity = quantity,
                    UnitPrice = product.Price
                });
            }
            else
            {
                item.Quantity += quantity;
            }

            await _unitOfWork.CompleteAsync();

            // reload to ensure product info in response
            cart = await _unitOfWork.Repository<Cart>()
                .GetByAsync(c => c.Id == cart.Id, includeProperties: CART_FULL);

            return _mapper.Map<CartResponse>(cart);
        }

        // --------------------------------------------------
        // UPDATE QUANTITY
        // --------------------------------------------------
        public async Task<CartResponse> UpdateItemQuantityAsync(Guid userId, Guid productId, int quantity)
        {
            var cart = await _unitOfWork.Repository<Cart>()
                .GetByAsync(c => c.UserId == userId, includeProperties: CART_FULL)
                ?? throw new KeyNotFoundException("Cart not found");

            var item = cart.CartItems.FirstOrDefault(i => i.ProductId == productId)
                ?? throw new KeyNotFoundException("Item not found");

            if (quantity <= 0)
            {
                await _unitOfWork.Repository<Cartitem>().DeleteAsync(item);
            }
            else
            {
                item.Quantity = quantity;
            }

            await _unitOfWork.CompleteAsync();

            cart = await _unitOfWork.Repository<Cart>()
                .GetByAsync(c => c.Id == cart.Id, includeProperties: CART_FULL);

            return _mapper.Map<CartResponse>(cart);
        }

        // --------------------------------------------------
        // REMOVE ITEM
        // --------------------------------------------------
        public async Task<bool> RemoveItemAsync(Guid userId, Guid productId)
        {
            var cart = await _unitOfWork.Repository<Cart>()
                .GetByAsync(c => c.UserId == userId, includeProperties: CART_ITEMS)
                ?? throw new KeyNotFoundException("Cart not found");

            var item = cart.CartItems.FirstOrDefault(i => i.ProductId == productId)
                ?? throw new KeyNotFoundException("Item not found");

            await _unitOfWork.Repository<Cartitem>().DeleteAsync(item);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        // --------------------------------------------------
        // CLEAR CART
        // --------------------------------------------------
        public async Task<bool> ClearCartAsync(Guid userId)
        {
            var cart = await _unitOfWork.Repository<Cart>()
                .GetByAsync(c => c.UserId == userId, includeProperties: CART_ITEMS)
                ?? throw new KeyNotFoundException("Cart not found");

            await _unitOfWork.Repository<Cartitem>().RemoveRangeAsync(cart.CartItems);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        // --------------------------------------------------
        // CHECK PRODUCT
        // --------------------------------------------------
        public async Task<bool> IsProductInCartAsync(Guid userId, Guid productId)
        {
            var cart = await _unitOfWork.Repository<Cart>()
                .GetByAsync(c => c.UserId == userId, includeProperties: CART_ITEMS);

            return cart?.CartItems.Any(i => i.ProductId == productId) ?? false;
        }

        // --------------------------------------------------
        // ITEMS COUNT
        // --------------------------------------------------
        public async Task<int> GetCartItemCountAsync(Guid userId)
        {
            var cart = await _unitOfWork.Repository<Cart>()
                .GetByAsync(c => c.UserId == userId, includeProperties: CART_ITEMS);

            return cart?.CartItems.Sum(i => i.Quantity) ?? 0;
        }

        // --------------------------------------------------
        // SUBTOTAL
        // --------------------------------------------------
        public async Task<decimal> GetCartSubtotalAsync(Guid userId)
        {
            var cart = await _unitOfWork.Repository<Cart>()
                .GetByAsync(c => c.UserId == userId, includeProperties: CART_FULL);

            return cart?.CartItems.Sum(i => i.Quantity * i.UnitPrice) ?? 0;
        }
    }
}
