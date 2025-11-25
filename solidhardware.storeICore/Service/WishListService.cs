using AutoMapper;
using Microsoft.Extensions.Logging;
using solidhardware.storeCore.Domain.Entites;
using solidhardware.storeCore.DTO.WishListDTO;
using solidhardware.storeCore.IUnitofWork;
using solidhardware.storeCore.ServiceContract;

public class WishlistService : IWishListService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<WishlistService> _logger;
    private readonly IMapper _mapper;

    private const string INCLUDES = "WishlistItems.Product";

    public WishlistService(IUnitOfWork unitOfWork,
                           ILogger<WishlistService> logger,
                           IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    // ------------------------------------------------------------
    // GET OR CREATE (Public)
    // ------------------------------------------------------------
    public async Task<WishListResponse> GetOrCreateAsync(Guid userId)
    {
        var wishlist = await GetOrCreateEntity(userId);

        return _mapper.Map<WishListResponse>(wishlist);
    }

    // ------------------------------------------------------------
    // ADD ITEM
    // ------------------------------------------------------------
    public async Task<WishListResponse> AddItemAsync(Guid userId, Guid productId)
    {
        var wishlist = await GetOrCreateEntity(userId);

        // Check Exists
        if (wishlist.WishlistItems.Any(x => x.ProductId == productId))
            throw new InvalidOperationException("Product already exists in wishlist.");

        var newItem = new WishlistItem
        {
            Id = Guid.NewGuid(),
            WishlistId = wishlist.Id,
            ProductId = productId
        };

        await _unitOfWork.Repository<WishlistItem>().CreateAsync(newItem);
        await _unitOfWork.CompleteAsync();

        // Reload with Include
        wishlist = await _unitOfWork.Repository<Wishlist>()
            .GetByAsync(w => w.Id == wishlist.Id, includeProperties: INCLUDES);

        return _mapper.Map<WishListResponse>(wishlist);
    }

    // ------------------------------------------------------------
    // REMOVE
    // ------------------------------------------------------------
    public async Task<bool> RemoveItemAsync(Guid userId, Guid productId)
    {
        var wishlist = await GetOrCreateEntity(userId);

        var item = wishlist.WishlistItems.FirstOrDefault(x => x.ProductId == productId);

        if (item == null)
            return false;

        await _unitOfWork.Repository<WishlistItem>().DeleteAsync(item);
        await _unitOfWork.CompleteAsync();

        return true;
    }

    // ------------------------------------------------------------
    // CLEAR
    // ------------------------------------------------------------
    public async Task<bool> ClearAsync(Guid userId)
    {
        var wishlist = await GetOrCreateEntity(userId);

        if (!wishlist.WishlistItems.Any())
            return true;

        await _unitOfWork.Repository<WishlistItem>().RemoveRangeAsync(wishlist.WishlistItems);
        await _unitOfWork.CompleteAsync();

        return true;
    }

    // ------------------------------------------------------------
    // GET BY USER
    // ------------------------------------------------------------
    public async Task<WishListResponse> GetByUserIdAsync(Guid userId)
    {
        var wishlist = await _unitOfWork.Repository<Wishlist>()
            .GetByAsync(w => w.UserId == userId, includeProperties: INCLUDES);

        if (wishlist == null)
            throw new KeyNotFoundException("Wishlist not found.");

        return _mapper.Map<WishListResponse>(wishlist);
    }

    // ------------------------------------------------------------
    // EXISTS
    // ------------------------------------------------------------
    public async Task<bool> IsInWishlistAsync(Guid userId, Guid productId)
    {
        var wishlist = await _unitOfWork.Repository<Wishlist>()
            .GetByAsync(w => w.UserId == userId, includeProperties: "WishlistItems");

        if (wishlist == null)
            return false;

        return wishlist.WishlistItems.Any(x => x.ProductId == productId);
    }


    // ------------------------------------------------------------
    // INTERNAL HELPER (FULLY FIXED)
    // ------------------------------------------------------------
    private async Task<Wishlist> GetOrCreateEntity(Guid userId)
    {
        // Step 1: Get without include first
        var wishlist = await _unitOfWork.Repository<Wishlist>()
            .GetByAsync(w => w.UserId == userId, isTracked: true);

        // Step 2: If found → reload with include
        if (wishlist != null)
        {
            wishlist = await _unitOfWork.Repository<Wishlist>()
                .GetByAsync(w => w.Id == wishlist.Id,
                            isTracked: true,
                            includeProperties: INCLUDES);

            return wishlist;
        }

        // Step 3: If not found → create
        wishlist = new Wishlist
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            WishlistItems = new List<WishlistItem>()
        };

        await _unitOfWork.Repository<Wishlist>().CreateAsync(wishlist);
        await _unitOfWork.CompleteAsync();

        // Step 4: Reload with include
        wishlist = await _unitOfWork.Repository<Wishlist>()
            .GetByAsync(w => w.Id == wishlist.Id,
                        isTracked: true,
                        includeProperties: INCLUDES);

        return wishlist;
    }
}
