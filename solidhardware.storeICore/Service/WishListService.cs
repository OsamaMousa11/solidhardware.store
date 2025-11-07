using AutoMapper;
using Microsoft.Extensions.Logging;
using solidhardware.storeCore.Domain.Entites;
using solidhardware.storeCore.Domain.IRepositoryContract;
using solidhardware.storeCore.DTO.ProductDTO;
using solidhardware.storeCore.DTO.WishListDTO;
using solidhardware.storeCore.Helper;
using solidhardware.storeCore.IUnitofWork;
using solidhardware.storeCore.ServiceContract;

namespace solidhardware.storeCore.Service
{
    public class WishlistService : IWishListService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<WishlistService> _logger;
        private readonly IWishListRepository _wishListRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICartService _cartService;

        public WishlistService(IMapper mapper , ILogger<WishlistService> logger , IWishListRepository wishListRepository, IUnitOfWork unitOfWork, ICartService cartService)
        {
            _logger=logger;
            _mapper=mapper;
            _unitOfWork=unitOfWork;
            _wishListRepository=wishListRepository;
            _cartService=cartService;
        }
        
     
        public  async Task<WishListResponse> AddItemAsync(Guid userId, Guid productId)
        {
            _logger.LogInformation("AddItem in WIshLisat");

            ValidateGuid(userId, nameof(userId));
            ValidateGuid(productId, nameof(productId));
          

            var wishlist = await _unitOfWork.Repository<Wishlist>()
                .GetByAsync(p => p.UserId == userId, includeProperties: "WishlistItems");
            if(wishlist == null)
            {
                               _logger.LogWarning("Wishlist for user {UserId} not found", userId);
                throw new KeyNotFoundException("Wishlist not found for the user");
            }
            var existingItem = wishlist.WishlistItems.FirstOrDefault(wi => wi.ProductId == productId);
            if(existingItem != null)
            {
                _logger.LogWarning("Product {ProductId} already exists in wishlist for user {UserId}", productId, userId);
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

            _logger.LogInformation("Product {ProductId} successfully added to wishlist for user {UserId}", productId, userId);

            var updatedWishlist = await _unitOfWork.Repository<Wishlist>()
                .GetByAsync(p => p.Id == wishlist.Id, includeProperties: "WishlistItems.Product.Brand,WishlistItems.Product.Images");
            return _mapper.Map<WishListResponse>(updatedWishlist);
        }

        public async Task<WishListResponse> AddWishlistAsync(WishListAddRequest request)
        {
            _logger.LogInformation("Adding item to wishlist");
            if (request == null)
            {
                _logger.LogError("WishlistItemAddRequest can not null");
                throw new ArgumentNullException(nameof(request));
            }

            ValidationHelper.ValidateModel(request);
            var existintwishlist = await _unitOfWork.Repository<Wishlist>().GetByAsync(p => p.UserId == request.UserId);
            if (existintwishlist != null)
            {
                _logger.LogWarning("Wishlist already exists for user {UserId}", request.UserId);
                throw new InvalidOperationException("A wishlist already exists for this user.");

            }

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var wishlist = _mapper.Map<Wishlist>(request);
                    wishlist.Id = Guid.NewGuid();
                    if (wishlist.WishlistItems != null && wishlist.WishlistItems.Count() > 0)
                    {
                        var wishlistsitems = request.WishlistItems
                                 .Select(sp =>
                                 {
                                     var wishlistresult = _mapper.Map<WishlistItem>(sp);
                                     wishlistresult.Id = Guid.NewGuid();
                                     wishlistresult.WishlistId = wishlist.Id;
                                     return wishlistresult;
                                 })
                                 .ToList();
                        wishlist.WishlistItems = wishlistsitems;
                    }
                    await _unitOfWork.Repository<Wishlist>().CreateAsync(wishlist);
                    await _unitOfWork.CompleteAsync();
                    await transaction.CommitAsync();

                    _logger.LogInformation("wishlist created successfully");

                    var createdwishlist = await _unitOfWork.Repository<Wishlist>()
                    .GetByAsync(p => p.Id == wishlist.Id, includeProperties: "WishlistItems.Product");


                    return _mapper.Map<WishListResponse>(createdwishlist);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "An error occurred while adding item to wishlist");
                    throw;
                }

            }
        }

        public async Task<bool> ClearUserWishlistAsync(Guid userId)
        {
            _logger.LogInformation("Clearing wishlist for user: {UserId}", userId);

            ValidateGuid(userId, nameof(userId));

            var wishlist = await _unitOfWork.Repository<Wishlist>()
                .GetByAsync(p => p.UserId == userId, includeProperties: "WishlistItems");

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
                    await _unitOfWork.Repository<WishlistItem>().RemoveRangeAsync(wishlist.WishlistItems);
                    await _unitOfWork.CompleteAsync();
                    await transaction.CommitAsync();

                    _logger.LogInformation("Wishlist for user {UserId} cleared successfully", userId);
                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error occurred while clearing wishlist for user {UserId}", userId);
                    throw;
                }
            }
        }

     

        public async Task<WishListResponse> GetByIdAsync(Guid wishlistId)
        {
            _logger.LogInformation("Getting wishlist by id: {WishlistId}", wishlistId);
            ValidateGuid(wishlistId, nameof(wishlistId));
            var wishlist = await _unitOfWork.Repository<Wishlist>()
                .GetByAsync(p => p.Id == wishlistId, includeProperties: "WishlistItems.Product.Brand,WishlistItems.Product.Images");
            if (wishlist == null)
            {
                _logger.LogWarning("Wishlist with id {WishlistId} not found", wishlistId);
                throw new KeyNotFoundException("Wishlist not found");
            }
            return _mapper.Map<WishListResponse>(wishlist);
        }

        public  async Task<WishListResponse> GetOrCreateWishlistAsync(Guid userId)
        {
            _logger.LogInformation("Getting or creating wishlist for user: {UserId}", userId);
            ValidateGuid(userId, nameof(userId));
            var wishlist = await _unitOfWork.Repository<Wishlist>()
                .GetByAsync(p => p.UserId == userId, includeProperties: "WishlistItems.Product.Brand,WishlistItems.Product.Images");
            if (wishlist != null)
                {
                _logger.LogInformation("Wishlist found for user: {UserId}", userId);
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
            _logger.LogInformation("Wishlist created for user: {UserId}", userId);
            return _mapper.Map<WishListResponse>(wishlist);

        }

        public async Task<WishListResponse> GetWishlistByUserIdAsync(Guid userId)
        {
            _logger.LogInformation("Getting wishlist for user: {UserId}", userId);

            ValidateGuid(userId, nameof(userId));

            var wishlist = await _unitOfWork.Repository<Wishlist>()
                .GetByAsync(p => p.UserId == userId, includeProperties: "WishlistItems.Product.Brand,WishlistItems.Product.Images");

            if (wishlist == null)
            {
                _logger.LogWarning("Wishlist for user {UserId} not found", userId);
                throw new KeyNotFoundException("Wishlist not found for the user");
            }
            return _mapper.Map<WishListResponse>(wishlist);
        }


        public async Task<bool> IsProductInWishlistAsync(Guid userId, Guid productId)
        {
            _logger.LogInformation("Checking if product {ProductId} is in wishlist for user {UserId}", productId, userId);

            ValidateGuid(userId, nameof(userId));
            ValidateGuid(productId, nameof(productId));


            var wishlist = await _unitOfWork.Repository<Wishlist>()
                .GetByAsync(p => p.UserId == userId, includeProperties: "WishlistItems");

            if (wishlist == null)
            {
                _logger.LogWarning("Wishlist for user {UserId} not found", userId);
                throw new KeyNotFoundException("Wishlist not found for the user");
            }

            var exists = wishlist.WishlistItems.Any(wi => wi.ProductId == productId);

            _logger.LogInformation("Product {ProductId} is {Status} in wishlist for user {UserId}",
                productId, exists ? "present" : "not present", userId);

            return exists;
        }

       public  async Task<bool>MoveAllToCartAsync(Guid userId)
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
                _logger.LogInformation(
                    "Wishlist is empty for user {UserId}, nothing to move",
                    userId);
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

                    // لف على كل الـ Items في الـ Wishlist
                    foreach (var item in wishlist.WishlistItems.ToList())
                    {
                        try
                        {
                            // تأكد إن المنتج محمل
                            if (item.Product == null)
                            {
                                _logger.LogWarning(
                                    "Product not loaded for wishlist item {ItemId}, skipping",
                                    item.Id);
                                failedCount++;
                                failedProducts.Add((item.ProductId, "Product data not loaded"));
                                continue;
                            }

                        
                            _logger.LogDebug(
                                "Attempting to add product {ProductId} to cart",
                                item.ProductId);

                            await _cartService.AddItemAsync(
                                userId,
                                item.ProductId,
                                quantity: 1); 

                  
                            itemsToRemove.Add(item);
                            movedCount++;

                            _logger.LogDebug(
                                "Product {ProductId} added to cart successfully",
                                item.ProductId);
                        }
                        catch (Exception ex)
                        {
                     
                            _logger.LogWarning(ex,
                                "Failed to move product {ProductId} to cart, continuing with others",
                                item.ProductId);

                            failedCount++;
                            failedProducts.Add((item.ProductId, ex.Message));
                        }
                    }

             
                    if (itemsToRemove.Any())
                    {
                        _logger.LogInformation(
                            "Removing {Count} items from wishlist",
                            itemsToRemove.Count);

                        await _unitOfWork.Repository<WishlistItem>()
                            .RemoveRangeAsync(itemsToRemove);
                    }

             
                    await _unitOfWork.CompleteAsync();
                    await transaction.CommitAsync();

                  
                    _logger.LogInformation(
                        "Move operation completed: {MovedCount} items moved successfully, {FailedCount} failed",
                        movedCount, failedCount);

                    if (failedProducts.Any())
                    {
                        _logger.LogWarning(
                            "Failed products: {@FailedProducts}",
                            failedProducts);
                    }

             
                    if (movedCount == 0 && failedCount > 0)
                    {
                        throw new InvalidOperationException(
                            $"Failed to move any items to cart. {failedCount} items failed.");
                    }

                 
                    if (failedCount > 0)
                    {
                        _logger.LogWarning(
                            "Partially successful: {MovedCount} moved, {FailedCount} failed",
                            movedCount, failedCount);
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex,
                        "Critical error while moving all wishlist items to cart for user {UserId}",
                        userId);
                    throw;
                }
            }



        }

        public async Task<bool> MoveItemToCartAsync(Guid userId, Guid wishlistItemId, int quantity = 1)
        {
            _logger.LogInformation(
                "Moving wishlist item {WishlistItemId} to cart for user {UserId}",
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
                _logger.LogWarning(
                    "Wishlist item {WishlistItemId} not found for user {UserId}",
                    wishlistItemId, userId);
                throw new KeyNotFoundException("Wishlist item not found");
            }


            if (wishlistItem.Product == null)
            {
                _logger.LogError(
                    "Product not loaded for wishlist item {WishlistItemId}",
                    wishlistItemId);
                throw new InvalidOperationException("Product data is missing");
            }


            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {

                    _logger.LogInformation(
                        "Adding product {ProductId} to cart with quantity {Quantity}",
                        wishlistItem.ProductId, quantity);

                    await _cartService.AddItemAsync(
                        userId,
                        wishlistItem.ProductId,
                        quantity);


                    _logger.LogInformation(
                        "Removing item {WishlistItemId} from wishlist",
                        wishlistItemId);

                    await _unitOfWork.Repository<WishlistItem>()
                        .DeleteAsync(wishlistItem);

                    await _unitOfWork.CompleteAsync();
                    await transaction.CommitAsync();

                    _logger.LogInformation(
                        "Successfully moved product {ProductId} from wishlist to cart for user {UserId}",
                        wishlistItem.ProductId, userId);

                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex,
                        "Error moving wishlist item {WishlistItemId} to cart for user {UserId}",
                        wishlistItemId, userId);
                    throw;
                }
            }
        }
        public  async Task<bool> RemoveItemAsync(Guid userId, Guid wishlistItemId)
        {
            _logger.LogInformation("Removing item {WishlistItemId} from wishlist for user {UserId}", wishlistItemId, userId);
            if (userId == Guid.Empty)
            {
                _logger.LogError("UserId cannot be empty");
                throw new ArgumentException("UserId cannot be empty", nameof(userId));
            }
            if (wishlistItemId == Guid.Empty)
            {
                _logger.LogError("WishlistItemId cannot be empty");
                throw new ArgumentException("WishlistItemId cannot be empty", nameof(wishlistItemId));
            }
            var wishlist = await _unitOfWork.Repository<Wishlist>()
                .GetByAsync(p => p.UserId == userId, includeProperties: "WishlistItems");
            if (wishlist == null)
            {
                _logger.LogWarning("Wishlist for user {UserId} not found", userId);
                throw new KeyNotFoundException("Wishlist not found for the user");
            }
            var itemToRemove = wishlist.WishlistItems.FirstOrDefault(wi => wi.Id == wishlistItemId);
            if (itemToRemove == null)
            {
                _logger.LogWarning("Wishlist item {WishlistItemId} not found for user {UserId}", wishlistItemId, userId);
                throw new KeyNotFoundException("Wishlist item not found");
            }
            await _unitOfWork.Repository<WishlistItem>().DeleteAsync(itemToRemove);
            await _unitOfWork.CompleteAsync();
            _logger.LogInformation("Wishlist item {WishlistItemId} removed successfully from wishlist for user {UserId}", wishlistItemId, userId);
            return true;

        }


        private void ValidateGuid(Guid id, string paramName)
        {
            if (id == Guid.Empty)
                throw new ArgumentException($"{paramName} cannot be empty", paramName);
            _logger.LogInformation("{ParamName} validated successfully", paramName);
        }




    }
}
