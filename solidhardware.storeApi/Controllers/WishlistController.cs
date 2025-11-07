using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using solidhardware.storeCore.DTO.WishListDTO;
using solidhardware.storeCore.Service;
using solidhardware.storeCore.ServiceContract;
using System;
using System.Threading.Tasks;

namespace solidhardware.storeApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WishlistController : ControllerBase
    {
        private readonly IWishListService _wishlistService;
        private readonly ILogger<WishlistController> _logger;

        public WishlistController(IWishListService wishlistService, ILogger<WishlistController> logger)
        {
            _wishlistService = wishlistService;
            _logger = logger;
        }

        /// <summary>
        /// Get or create wishlist for a user
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetOrCreateWishlist(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                _logger.LogWarning("Invalid user ID provided for GetOrCreateWishlist");
                return BadRequest("Invalid user ID.");
            }

            _logger.LogInformation("Getting or creating wishlist for user {UserId}", userId);

            var wishlist = await _wishlistService.GetOrCreateWishlistAsync(userId);

            _logger.LogInformation("Wishlist retrieved successfully for user {UserId}", userId);
            return Ok(wishlist);
        }

        /// <summary>
        /// Add a product to wishlist
        /// </summary>
        [HttpPost("add")]
        public async Task<IActionResult> AddToWishlist([FromBody] WishListAddRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for AddToWishlist: {@Request}", request);
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Adding product {ProductId} to wishlist for user {UserId}", request.ProductId, request.UserId);

            var updatedWishlist = await _wishlistService.AddItemAsync(request.UserId, request.ProductId);

            _logger.LogInformation("Product {ProductId} added to wishlist for user {UserId}", request.ProductId, request.UserId);
            return Ok(updatedWishlist);
        }

        /// <summary>
        /// Remove product from wishlist
        /// </summary>
        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveFromWishlist([FromQuery] Guid userId, [FromQuery] Guid productId)
        {
            if (userId == Guid.Empty || productId == Guid.Empty)
            {
                _logger.LogWarning("Invalid parameters for RemoveFromWishlist. userId={UserId}, productId={ProductId}", userId, productId);
                return BadRequest("Invalid parameters.");
            }

            _logger.LogInformation("Removing product {ProductId} from wishlist for user {UserId}", productId, userId);

            var result = await _wishlistService.RemoveItemAsync(userId, productId);
            if (!result)
            {
                _logger.LogWarning("Product {ProductId} not found in wishlist for user {UserId}", productId, userId);
                return NotFound("Product not found in wishlist.");
            }

            _logger.LogInformation("Product {ProductId} removed successfully from wishlist for user {UserId}", productId, userId);
            return Ok("Product removed from wishlist.");
        }

        /// <summary>
        /// Clear all items from wishlist
        /// </summary>
        [HttpDelete("clear/{userId}")]
        public async Task<IActionResult> ClearWishlist(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                _logger.LogWarning("Invalid user ID provided for ClearWishlist");
                return BadRequest("Invalid user ID.");
            }

            _logger.LogInformation("Clearing wishlist for user {UserId}", userId);

            var result = await _wishlistService.ClearUserWishlistAsync(userId);
            if (!result)
            {
                _logger.LogWarning("Wishlist not found for user {UserId}", userId);
                return NotFound("Wishlist not found.");
            }

            _logger.LogInformation("Wishlist cleared successfully for user {UserId}", userId);
            return Ok("Wishlist cleared successfully.");
        }

        /// <summary>
        /// Get all items in wishlist
        /// </summary>
        [HttpGet("{userId}/items")]
        public async Task<IActionResult> GetWishlistItems(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                _logger.LogWarning("Invalid user ID provided for GetWishlistItems");
                return BadRequest("Invalid user ID.");
            }

            _logger.LogInformation("Fetching wishlist items for user {UserId}", userId);

            var wishlist = await _wishlistService.GetWishlistByUserIdAsync(userId);
            if (wishlist == null)
            {
                _logger.LogWarning("Wishlist not found for user {UserId}", userId);
                return NotFound("Wishlist not found.");
            }

            _logger.LogInformation("Wishlist items fetched successfully for user {UserId}", userId);
            return Ok(wishlist);
        }

     
     
    }
}
