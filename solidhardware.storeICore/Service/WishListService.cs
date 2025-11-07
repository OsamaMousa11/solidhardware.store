using AutoMapper;
using Microsoft.Extensions.Logging;
using solidhardware.storeCore.Domain.Entites;
using solidhardware.storeCore.Domain.IRepositoryContract;
using solidhardware.storeCore.DTO.WishListDTO;
using solidhardware.storeCore.Helper;
using solidhardware.storeCore.IUnitofWork;
using solidhardware.storeCore.ServiceContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace solidhardware.storeCore.Service
{
    public class WishlistService : IWishListService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<WishlistService> _logger;
        private readonly IWishListRepository _wishListRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICartService _cartService;

      
        private const string WISHLIST_BASIC_INCLUDES = "WishlistItems";
        private const string WISHLIST_FULL_INCLUDES = "WishlistItems.Product.Brand,WishlistItems.Product.Images";

        public WishlistService(
            IMapper mapper,
            ILogger<WishlistService> logger,
            IWishListRepository wishListRepository,
            IUnitOfWork unitOfWork,
            ICartService cartService)
        {
            _logger = logger;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _wishListRepository = wishListRepository;
            _cartService = cartService;
        }

        /// <summary>
        /// Adds a product to user's wishlist
        /// </summary>
        public async Task<WishListResponse> AddItemAsync(Guid userId, Guid productId)
        {
            _logger.LogInformation("Adding product {ProductId} to wishlist for user {UserId}",
                productId, userId);

            ValidateGuid(userId, nameof(userId));
            ValidateGuid(productId, nameof(productId));

        
            var wishlist = await _unitOfWork.Repository<Wishlist>()
                .GetByAsync(
                    p => p.UserId == userId,
                    includeProperties: WISHLIST_FULL_INCLUDES);

            if (wishlist == null)
            {
                _logger.LogWarning("Wishlist for user {UserId} not found", userId);
                throw new KeyNotFoundException("Wishlist not found for the user");
            }

     
            var existingItem = wishlist.WishlistItems
                .FirstOrDefault(wi => wi.ProductId == productId);

            if (existingItem != null)
            {
                _logger.LogWarning("Product {ProductId} already exists in wishlist for user {UserId}",
                    productId, userId);
                throw new InvalidOperationException("Product already exists in the wishlist");
            }

            var wishlistItem = new WishlistItem
            {
                Id = Guid.NewGuid(),
                WishlistId = wishlist.Id,
                ProductId = productId,

            };

            await _unitOfWork.Repository<WishlistItem>().CreateAsync(wishlistItem);
            await _unitOfWork.CompleteAsync();

     
            wishlist.WishlistItems.Add(wishlistItem);

            _logger.LogInformation("Product {ProductId} successfully added to wishlist for user {UserId}",
                productId, userId);

            return _mapper.Map<WishListResponse>(wishlist);
        }

        /// <summary>
        /// Creates a new wishlist for a user
        /// </summary>
        public async Task<WishListResponse> AddWishlistAsync(WishListAddRequest request)
        {
            _logger.LogInformation("Creating wishlist for user {UserId}", request?.UserId);

            if (request == null)
            {
                _logger.LogError("WishlistAddRequest cannot be null");
                throw new ArgumentNullException(nameof(request));
            }

            ValidationHelper.ValidateModel(request);

            var existingWishlist = await _unitOfWork.Repository<Wishlist>()
                .GetByAsync(p => p.UserId == request.UserId);

            if (existingWishlist != null)
            {
                _logger.LogWarning("Wishlist already exists for user {UserId}", request.UserId);
                throw new InvalidOperationException("A wishlist already exists for this user");
            }

            var wishlist = _mapper.Map<Wishlist>(request);
            wishlist.Id = Guid.NewGuid();
        

            if (wishlist.WishlistItems != null && wishlist.WishlistItems.Any())
            {
                var wishlistItems = request.WishlistItems
                    .Select(item =>
                    {
                        var mappedItem = _mapper.Map<WishlistItem>(item);
                        mappedItem.Id = Guid.NewGuid();
                        mappedItem.WishlistId = wishlist.Id;
            
                        return mappedItem;
                    })
                    .ToList();
                wishlist.WishlistItems = wishlistItems;
            }

            await _unitOfWork.Repository<Wishlist>().CreateAsync(wishlist);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Wishlist created successfully for user {UserId}", request.UserId);

            var createdWishlist = await _unitOfWork.Repository<Wishlist>()
                .GetByAsync(
                    p => p.Id == wishlist.Id,
                    includeProperties: WISHLIST_FULL_INCLUDES);

            return _mapper.Map<WishListResponse>(createdWishlist);
        }

        /// <summary>
        /// Clears all items from user's wishlist
        /// </summary>
        public async Task<bool> ClearUserWishlistAsync(Guid userId)
        {
            _logger.LogInformation("Clearing wishlist for user {UserId}", userId);

            ValidateGuid(userId, nameof(userId));

            var wishlist = await _unitOfWork.Repository<Wishlist>()
                .GetByAsync(
                    p => p.UserId == userId,
                    includeProperties: WISHLIST_BASIC_INCLUDES);

            if (wishlist == null)
            {
                _logger.LogWarning("Wishlist for user {UserId} not found", userId);
                throw new KeyNotFoundException("Wishlist not found for the user");
            }

            if (wishlist.WishlistItems == null || !wishlist.WishlistItems.Any())
            {
                _logger.LogInformation("Wishlist for user {UserId} is already empty", userId);
                return true;
            }

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    await _unitOfWork.Repository<WishlistItem>()
                        .RemoveRangeAsync(wishlist.WishlistItems);


                    await _wishListRepository.UpdateWishlistAsync(wishlist);

                    await _unitOfWork.CompleteAsync();
                    await transaction.CommitAsync();

                    _logger.LogInformation("Wishlist for user {UserId} cleared successfully", userId);
                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error clearing wishlist for user {UserId}", userId);
                    throw;
                }
            }
        }

        /// <summary>
        /// Gets wishlist by ID
        /// </summary>
        public async Task<WishListResponse> GetByIdAsync(Guid wishlistId)
        {
            _logger.LogInformation("Getting wishlist by id {WishlistId}", wishlistId);

            ValidateGuid(wishlistId, nameof(wishlistId));

            var wishlist = await _unitOfWork.Repository<Wishlist>()
                .GetByAsync(
                    p => p.Id == wishlistId,
                    includeProperties: WISHLIST_FULL_INCLUDES);

            if (wishlist == null)
            {
                _logger.LogWarning("Wishlist {WishlistId} not found", wishlistId);
                throw new KeyNotFoundException("Wishlist not found");
            }

            return _mapper.Map<WishListResponse>(wishlist);
        }

        /// <summary>
        /// Gets existing wishlist or creates new one for user
        /// </summary>
        public async Task<WishListResponse> GetOrCreateWishlistAsync(Guid userId)
        {
            _logger.LogInformation("Getting or creating wishlist for user {UserId}", userId);

            ValidateGuid(userId, nameof(userId));

            var wishlist = await _unitOfWork.Repository<Wishlist>()
                .GetByAsync(
                    p => p.UserId == userId,
                    includeProperties: WISHLIST_FULL_INCLUDES);

            if (wishlist != null)
            {
                _logger.LogInformation("Wishlist found for user {UserId}", userId);
                return _mapper.Map<WishListResponse>(wishlist);
            }

            wishlist = new Wishlist
            {
                Id = Guid.NewGuid(),
                UserId = userId,
              
                WishlistItems = new List<WishlistItem>()
            };

            await _unitOfWork.Repository<Wishlist>().CreateAsync(wishlist);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Wishlist created for user {UserId}", userId);

            return _mapper.Map<WishListResponse>(wishlist);
        }

        /// <summary>
        /// Gets wishlist for specific user
        /// </summary>
        public async Task<WishListResponse> GetWishlistByUserIdAsync(Guid userId)
        {
            _logger.LogInformation("Getting wishlist for user {UserId}", userId);

            ValidateGuid(userId, nameof(userId));

            var wishlist = await _unitOfWork.Repository<Wishlist>()
                .GetByAsync(
                    p => p.UserId == userId,
                    includeProperties: WISHLIST_FULL_INCLUDES);

            if (wishlist == null)
            {
                _logger.LogWarning("Wishlist for user {UserId} not found", userId);
                throw new KeyNotFoundException("Wishlist not found for the user");
            }

            return _mapper.Map<WishListResponse>(wishlist);
        }

        /// <summary>
        /// Checks if product exists in user's wishlist
        /// </summary>
        public async Task<bool> IsProductInWishlistAsync(Guid userId, Guid productId)
        {
            _logger.LogInformation("Checking if product {ProductId} is in wishlist for user {UserId}",
                productId, userId);

            ValidateGuid(userId, nameof(userId));
            ValidateGuid(productId, nameof(productId));

            var wishlist = await _unitOfWork.Repository<Wishlist>()
                .GetByAsync(
                    p => p.UserId == userId,
                    includeProperties: WISHLIST_BASIC_INCLUDES);

            if (wishlist == null)
            {
                _logger.LogWarning("Wishlist for user {UserId} not found", userId);
                return false; // Return false بدل exception
            }

            var exists = wishlist.WishlistItems.Any(wi => wi.ProductId == productId);

            _logger.LogInformation("Product {ProductId} is {Status} in wishlist for user {UserId}",
                productId, exists ? "present" : "not present", userId);

            return exists;
        }

        /// <summary>
        /// Moves all wishlist items to cart
        /// </summary>
        public async Task<bool> MoveAllToCartAsync(Guid userId)
        {
            _logger.LogInformation("Moving all wishlist items to cart for user {UserId}", userId);

            ValidateGuid(userId, nameof(userId));

            var wishlist = await _unitOfWork.Repository<Wishlist>()
                .GetByAsync(
                    w => w.UserId == userId,
                    includeProperties: "WishlistItems.Product");

            if (wishlist == null)
            {
                _logger.LogWarning("Wishlist not found for user {UserId}", userId);
                throw new KeyNotFoundException("Wishlist not found for the user");
            }

            if (wishlist.WishlistItems == null || !wishlist.WishlistItems.Any())
            {
                _logger.LogInformation("Wishlist is empty for user {UserId}", userId);
                return true;
            }

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    int movedCount = 0;
                    int failedCount = 0;
                    var itemsToRemove = new List<WishlistItem>();
                    var failedProducts = new List<(Guid ProductId, string Error)>();

                    foreach (var item in wishlist.WishlistItems.ToList())
                    {
                        try
                        {
                            if (item.Product == null)
                            {
                                _logger.LogWarning("Product not loaded for wishlist item {ItemId}", item.Id);
                                failedCount++;
                                failedProducts.Add((item.ProductId, "Product data not loaded"));
                                continue;
                            }

                            await _cartService.AddItemAsync(userId, item.ProductId, quantity: 1);

                            itemsToRemove.Add(item);
                            movedCount++;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to move product {ProductId} to cart",
                                item.ProductId);
                            failedCount++;
                            failedProducts.Add((item.ProductId, ex.Message));
                        }
                    }

                    if (itemsToRemove.Any())
                    {
                        await _unitOfWork.Repository<WishlistItem>()
                            .RemoveRangeAsync(itemsToRemove);


                        await _wishListRepository.UpdateWishlistAsync(wishlist);
                    }

                    await _unitOfWork.CompleteAsync();
                    await transaction.CommitAsync();

                    _logger.LogInformation(
                        "Move operation completed: {MovedCount} moved, {FailedCount} failed",
                        movedCount, failedCount);

                    if (failedProducts.Any())
                    {
                        _logger.LogWarning("Failed products: {@FailedProducts}", failedProducts);
                    }

                    if (movedCount == 0 && failedCount > 0)
                    {
                        throw new InvalidOperationException(
                            $"Failed to move any items to cart. {failedCount} items failed");
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error moving all wishlist items to cart for user {UserId}",
                        userId);
                    throw;
                }
            }
        }

        /// <summary>
        /// Moves a single item from wishlist to cart
        /// </summary>
        public async Task<bool> MoveItemToCartAsync(Guid userId, Guid wishlistItemId, int quantity = 1)
        {
            _logger.LogInformation("Moving wishlist item {WishlistItemId} to cart for user {UserId}",
                wishlistItemId, userId);

            ValidateGuid(userId, nameof(userId));
            ValidateGuid(wishlistItemId, nameof(wishlistItemId));

            if (quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));
            }

            var wishlist = await _unitOfWork.Repository<Wishlist>()
                .GetByAsync(
                    w => w.UserId == userId,
                    includeProperties: "WishlistItems.Product");

            if (wishlist == null)
            {
                _logger.LogWarning("Wishlist not found for user {UserId}", userId);
                throw new KeyNotFoundException("Wishlist not found for the user");
            }

            var wishlistItem = wishlist.WishlistItems
                .FirstOrDefault(wi => wi.Id == wishlistItemId);

            if (wishlistItem == null)
            {
                _logger.LogWarning("Wishlist item {WishlistItemId} not found", wishlistItemId);
                throw new KeyNotFoundException("Wishlist item not found");
            }

            if (wishlistItem.Product == null)
            {
                _logger.LogError("Product not loaded for wishlist item {WishlistItemId}", wishlistItemId);
                throw new InvalidOperationException("Product data is missing");
            }

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    await _cartService.AddItemAsync(userId, wishlistItem.ProductId, quantity);

                    await _unitOfWork.Repository<WishlistItem>().DeleteAsync(wishlistItem);


                    await _wishListRepository.UpdateWishlistAsync(wishlist);

                    await _unitOfWork.CompleteAsync();
                    await transaction.CommitAsync();

                    _logger.LogInformation("Successfully moved product {ProductId} from wishlist to cart",
                        wishlistItem.ProductId);

                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error moving wishlist item {WishlistItemId} to cart",
                        wishlistItemId);
                    throw;
                }
            }
        }

        /// <summary>
        /// Removes an item from wishlist
        /// </summary>
        public async Task<bool> RemoveItemAsync(Guid userId, Guid wishlistItemId)
        {
            _logger.LogInformation("Removing item {WishlistItemId} from wishlist for user {UserId}",
                wishlistItemId, userId);

            ValidateGuid(userId, nameof(userId));
            ValidateGuid(wishlistItemId, nameof(wishlistItemId));

            var wishlist = await _unitOfWork.Repository<Wishlist>()
                .GetByAsync(
                    p => p.UserId == userId,
                    includeProperties: WISHLIST_BASIC_INCLUDES);

            if (wishlist == null)
            {
                _logger.LogWarning("Wishlist for user {UserId} not found", userId);
                throw new KeyNotFoundException("Wishlist not found for the user");
            }

            var itemToRemove = wishlist.WishlistItems
                .FirstOrDefault(wi => wi.Id == wishlistItemId);

            if (itemToRemove == null)
            {
                _logger.LogWarning("Wishlist item {WishlistItemId} not found", wishlistItemId);
                throw new KeyNotFoundException("Wishlist item not found");
            }

            await _unitOfWork.Repository<WishlistItem>().DeleteAsync(itemToRemove);

           
            
            await _wishListRepository.UpdateWishlistAsync(wishlist);


            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Wishlist item {WishlistItemId} removed successfully", wishlistItemId);

            return true;
        }

     
        private void ValidateGuid(Guid id, string paramName)
        {
            if (id == Guid.Empty)
            {
                _logger.LogError("{ParamName} cannot be empty", paramName);
                throw new ArgumentException($"{paramName} cannot be empty", paramName);
            }
        }
    }
}