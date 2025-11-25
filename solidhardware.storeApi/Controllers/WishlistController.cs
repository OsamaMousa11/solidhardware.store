using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using solidhardware.storeCore.DTO;
using solidhardware.storeCore.DTO.WishListDTO;
using solidhardware.storeCore.ServiceContract;
using System.Net;
using System.Security.Claims;

namespace solidhardware.storeApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // المستخدم لازم يكون عامل Login
    public class WishlistController : ControllerBase
    {
        private readonly IWishListService _wishlistService;
        private readonly ILogger<WishlistController> _logger;

        public WishlistController(IWishListService wishlistService, ILogger<WishlistController> logger)
        {
            _wishlistService = wishlistService;
            _logger = logger;
        }

        // Helper: نجيب userId من التوكن
        private Guid GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.Parse(userId);
        }


        // ------------------------------------------------------------
        // GET OR CREATE WISHLIST
        // ------------------------------------------------------------
        [HttpGet("get")]
        public async Task<ActionResult<ApiResponse>> GetOrCreateWishlist()
        {
            try
            {
                var userId = GetUserId();
                _logger.LogInformation("GetOrCreateWishlist called by {UserId}", userId);

                var wishlist = await _wishlistService.GetOrCreateAsync(userId);

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Wishlist retrieved successfully",
                    Result = wishlist,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetOrCreateWishlist");
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Messages = "Error retrieving wishlist",
                    StatusCode = HttpStatusCode.InternalServerError
                });
            }
        }



        // ------------------------------------------------------------
        // ADD ITEM
        // ------------------------------------------------------------
        [HttpPost("add")]
        public async Task<ActionResult<ApiResponse>> AddToWishlist([FromBody] WishListAddRequest request)
        {
            try
            {
                var userId = GetUserId();

                if (request == null || request.ProductId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse
                    {
                        IsSuccess = false,
                        Messages = "Invalid request.",
                        StatusCode = HttpStatusCode.BadRequest
                    });
                }

                _logger.LogInformation("Adding product {ProductId} to wishlist for {UserId}",
                    request.ProductId, userId);

                var wishlist = await _wishlistService.AddItemAsync(userId, request.ProductId);

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Product added to wishlist",
                    Result = wishlist,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (InvalidOperationException ex) // لو المنتج موجود قبل كده
            {
                _logger.LogWarning(ex, "AddToWishlist warning");

                return BadRequest(new ApiResponse
                {
                    IsSuccess = false,
                    Messages = ex.Message,
                    StatusCode = HttpStatusCode.BadRequest
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddToWishlist");

                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Messages = "Error adding product to wishlist",
                    StatusCode = HttpStatusCode.InternalServerError
                });
            }
        }



        // ------------------------------------------------------------
        // REMOVE ITEM
        // ------------------------------------------------------------
        [HttpDelete("remove/{productId}")]
        public async Task<ActionResult<ApiResponse>> RemoveFromWishlist(Guid productId)
        {
            try
            {
                var userId = GetUserId();

                if (productId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse
                    {
                        IsSuccess = false,
                        Messages = "Invalid productId",
                        StatusCode = HttpStatusCode.BadRequest
                    });
                }

                _logger.LogInformation("Removing product {ProductId} from wishlist for {UserId}",
                    productId, userId);

                var result = await _wishlistService.RemoveItemAsync(userId, productId);

                if (!result)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Messages = "Product not found in wishlist",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Product removed from wishlist",
                    Result = null,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RemoveFromWishlist");

                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Messages = "Error removing product from wishlist",
                    StatusCode = HttpStatusCode.InternalServerError
                });
            }
        }



        // ------------------------------------------------------------
        // CLEAR WISHLIST
        // ------------------------------------------------------------
        [HttpDelete("clear")]
        public async Task<ActionResult<ApiResponse>> ClearWishlist()
        {
            try
            {
                var userId = GetUserId();

                _logger.LogInformation("Clearing wishlist for {UserId}", userId);

                await _wishlistService.ClearAsync(userId);

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Wishlist cleared successfully",
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ClearWishlist");

                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Messages = "Error clearing wishlist",
                    StatusCode = HttpStatusCode.InternalServerError
                });
            }
        }


        // ------------------------------------------------------------
        // GET ALL ITEMS
        // ------------------------------------------------------------
        [HttpGet("items")]
        public async Task<ActionResult<ApiResponse>> GetWishlistItems()
        {
            try
            {
                var userId = GetUserId();

                _logger.LogInformation("Getting wishlist items for {UserId}", userId);

                var wishlist = await _wishlistService.GetByUserIdAsync(userId);

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Wishlist items retrieved successfully",
                    Result = wishlist,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetWishlistItems");

                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Messages = "Error fetching wishlist items",
                    StatusCode = HttpStatusCode.InternalServerError
                });
            }
        }
    }
}
