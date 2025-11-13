using Microsoft.AspNetCore.Mvc;
using solidhardware.storeCore.DTO;
using solidhardware.storeCore.DTO.CartDTO;
using solidhardware.storeCore.ServiceContract;
using System.Net;

namespace solidhardware.storeApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CartController> _logger;

        public CartController(ILogger<CartController> logger, ICartService cartService)
        {
            _logger = logger;
            _cartService = cartService;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<ApiResponse>> GetCart(Guid userId)
        {
            try
            {
                var cart = await _cartService.GetCartByUserIdAsync(userId);
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Cart retrieved successfully",
                    Result = cart,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ApiResponse
                {
                    IsSuccess = false,
                    Messages = "Cart not found for user",
                    StatusCode = HttpStatusCode.NotFound
                });
            }
        }

        [HttpPost("additem")]
        public async Task<ActionResult<ApiResponse>> AddItemToCart([FromBody] CartAddRequest request)
        {
            try
            {
                var response = await _cartService.AddItemAsync(request.UserId, request.ProductId, request.Quantity);
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Item added successfully",
                    Result = response,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item for user {UserId}", request.UserId);
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Messages = "Error adding item to cart",
                    StatusCode = HttpStatusCode.InternalServerError
                });
            }
        }

        [HttpPost("updateitem")]
        public async Task<ActionResult<ApiResponse>> UpdateItemQuantity([FromBody] CartUpdateRequest request)
        {
            try
            {
                var result = await _cartService.UpdateItemQuantityAsync(request);
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Item quantity updated successfully",
                    Result = result,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating item in cart for user {UserId}", request.UserId);
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Messages = "Error updating item quantity",
                    StatusCode = HttpStatusCode.InternalServerError
                });
            }
        }

        [HttpDelete("removeitem/{userId}/{cartItemId}")]
        public async Task<ActionResult<ApiResponse>> RemoveItem(Guid userId, Guid cartItemId)
        {
            try
            {
                var result = await _cartService.RemoveItemAsync(userId, cartItemId);
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Item removed from cart",
                    Result = result,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing item from cart for user {UserId}", userId);
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Messages = "Error removing item",
                    StatusCode = HttpStatusCode.InternalServerError
                });
            }
        }

        [HttpDelete("clear/{userId}")]
        public async Task<ActionResult<ApiResponse>> ClearCart(Guid userId)
        {
            try
            {
                var result = await _cartService.ClearCartAsync(userId);
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Cart cleared successfully",
                    Result = result,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cart for user {UserId}", userId);
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Messages = "Error clearing cart",
                    StatusCode = HttpStatusCode.InternalServerError
                });
            }
        }

        [HttpGet("items/{userId}")]
        public async Task<ActionResult<ApiResponse>> GetCartItems(Guid userId)
        {
            var items = await _cartService.GetCartItemsAsync(userId);
            return Ok(new ApiResponse
            {
                IsSuccess = true,
                Messages = "Cart items retrieved successfully",
                Result = items,
                StatusCode = HttpStatusCode.OK
            });
        }

        [HttpGet("itemcount/{userId}")]
        public async Task<ActionResult<ApiResponse>> GetCartItemCount(Guid userId)
        {
            var count = await _cartService.GetCartItemCountAsync(userId);
            return Ok(new ApiResponse
            {
                IsSuccess = true,
                Messages = "Cart item count retrieved successfully",
                Result = count,
                StatusCode = HttpStatusCode.OK
            });
        }

        [HttpGet("total/{userId}")]
        public async Task<ActionResult<ApiResponse>> GetCartTotal(Guid userId)
        {
            var total = await _cartService.GetCartTotalAsync(userId);
            return Ok(new ApiResponse
            {
                IsSuccess = true,
                Messages = "Cart total retrieved successfully",
                Result = total,
                StatusCode = HttpStatusCode.OK
            });
        }

        [HttpGet("contains/{userId}/{productId}")]
        public async Task<ActionResult<ApiResponse>> IsProductInCart(Guid userId, Guid productId)
        {
            var exists = await _cartService.IsProductInCartAsync(userId, productId);
            return Ok(new ApiResponse
            {
                IsSuccess = true,
                Messages = "Product check completed",
                Result = exists,
                StatusCode = HttpStatusCode.OK
            });
        }
    }
}
