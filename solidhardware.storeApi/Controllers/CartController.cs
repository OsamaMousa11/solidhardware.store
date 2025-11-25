using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using solidhardware.storeCore.DTO;
using solidhardware.storeCore.DTO.CartDTO;
using solidhardware.storeCore.ServiceContract;
using System.Net;

namespace solidhardware.storeApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // كل الأكشنز هنا تتطلب USER (أو Admin)
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CartController> _logger;

        public CartController(ILogger<CartController> logger, ICartService cartService)
        {
            _logger = logger;
            _cartService = cartService;
        }

        // ----------------------------------------------------
        // GET CART
        // ----------------------------------------------------
        [HttpGet("{userId}")]
        public async Task<ActionResult<ApiResponse>> GetCart(Guid userId)
        {
            try
            {
                var cart = await _cartService.GetCartAsync(userId);

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Cart retrieved successfully",
                    Result = cart,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cart");
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Messages = "Error retrieving cart",
                    StatusCode = HttpStatusCode.InternalServerError
                });
            }
        }

        // ----------------------------------------------------
        // ADD OR UPDATE ITEM
        // ----------------------------------------------------
        [HttpPost("add")]
        public async Task<ActionResult<ApiResponse>> AddItem([FromBody] CartAddRequest request)
        {
            try
            {
                var result = await _cartService.AddOrUpdateItemAsync(
                    request.UserId,
                    request.ProductId,
                    request.Quantity);

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Item added/updated successfully",
                    Result = result,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding/updating cart item");
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Messages = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError
                });
            }
        }

        // ----------------------------------------------------
        // UPDATE ITEM QUANTITY
        // ----------------------------------------------------
        [HttpPost("update")]
        public async Task<ActionResult<ApiResponse>> UpdateQuantity([FromBody] CartUpdateRequest request)
        {
            try
            {
                var updated = await _cartService.UpdateItemQuantityAsync(
                    request.UserId,
                    request.ProductId,
                    request.Quantity);

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Quantity updated successfully",
                    Result = updated,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating quantity");
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Messages = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError
                });
            }
        }

        // ----------------------------------------------------
        // REMOVE ITEM
        // ----------------------------------------------------
        [HttpDelete("remove/{userId}/{productId}")]
        public async Task<ActionResult<ApiResponse>> RemoveItem(Guid userId, Guid productId)
        {
            try
            {
                var result = await _cartService.RemoveItemAsync(userId, productId);

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Item removed successfully",
                    Result = result,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cart item");
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Messages = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError
                });
            }
        }

        // ----------------------------------------------------
        // CLEAR CART
        // ----------------------------------------------------
        [HttpDelete("clear/{userId}")]
        public async Task<ActionResult<ApiResponse>> Clear(Guid userId)
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
                _logger.LogError(ex, "Error clearing cart");
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Messages = "Error clearing cart",
                    StatusCode = HttpStatusCode.InternalServerError
                });
            }
        }

        // ----------------------------------------------------
        // CHECK IF PRODUCT EXISTS IN CART
        // ----------------------------------------------------
        [HttpGet("contains/{userId}/{productId}")]
        public async Task<ActionResult<ApiResponse>> Contains(Guid userId, Guid productId)
        {
            try
            {
                var exists = await _cartService.IsProductInCartAsync(userId, productId);

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Check completed",
                    Result = exists,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking product in cart");
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Messages = "Internal server error",
                    StatusCode = HttpStatusCode.InternalServerError
                });
            }
        }

        // ----------------------------------------------------
        // COUNT ITEMS IN CART
        // ----------------------------------------------------
        [HttpGet("count/{userId}")]
        public async Task<ActionResult<ApiResponse>> Count(Guid userId)
        {
            try
            {
                var count = await _cartService.GetCartItemCountAsync(userId);

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Item count retrieved",
                    Result = count,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting cart items");
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Messages = "Error retrieving count",
                    StatusCode = HttpStatusCode.InternalServerError
                });
            }
        }

        // ----------------------------------------------------
        // SUBTOTAL
        // ----------------------------------------------------
        [HttpGet("subtotal/{userId}")]
        public async Task<ActionResult<ApiResponse>> Subtotal(Guid userId)
        {
            try
            {
                var total = await _cartService.GetCartSubtotalAsync(userId);

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Subtotal retrieved",
                    Result = total,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating subtotal");
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Messages = "Error calculating subtotal",
                    StatusCode = HttpStatusCode.InternalServerError
                });
            }
        }
    }
}
