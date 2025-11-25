using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using solidhardware.storeCore.DTO;
using solidhardware.storeCore.DTO.OrderDTO;
using solidhardware.storeCore.ServiceContract;
using System.Net;

namespace solidhardware.storeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]  // كل الAPI دي محتاجة User على الأقل
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderService orderService, ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }


        // -----------------------------------------------------------
        // CREATE ORDER  (USER ROLE)
        // -----------------------------------------------------------
        [HttpPost("create")]
        public async Task<ActionResult<ApiResponse>> CreateOrder([FromBody] OrderAddRequest request)
        {
            try
            {
                var order = await _orderService.CreateOrderAsync(request);

                return StatusCode(201, new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Order created successfully",
                    Result = order,
                    StatusCode = HttpStatusCode.Created
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");

                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Messages = "Error creating order",
                    StatusCode = HttpStatusCode.InternalServerError
                });
            }
        }


        // -----------------------------------------------------------
        // UPDATE ORDER  (ADMIN ONLY)
        // -----------------------------------------------------------
        [Authorize(Roles = "ADMIN")]
        [HttpPut("update")]
        public async Task<ActionResult<ApiResponse>> UpdateOrder([FromBody] OrderUpdateRequest request)
        {
            try
            {
                var order = await _orderService.UpdateOrderAsync(request);

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Order updated successfully",
                    Result = order,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order");

                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Messages = "Error updating order",
                    StatusCode = HttpStatusCode.InternalServerError
                });
            }
        }


        // -----------------------------------------------------------
        // DELETE ORDER  (ADMIN ONLY)
        // -----------------------------------------------------------
        [Authorize(Roles = "ADMIN")]
        [HttpDelete("delete/{orderId}")]
        public async Task<ActionResult<ApiResponse>> DeleteOrder(Guid orderId)
        {
            try
            {
                var result = await _orderService.DeleteOrderAsync(orderId);

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = result ? "Order deleted successfully" : "Order not found",
                    Result = result,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting order");

                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Messages = "Error deleting order",
                    StatusCode = HttpStatusCode.InternalServerError
                });
            }
        }


        // -----------------------------------------------------------
        // GET ORDER BY ID (ADMIN ONLY OR OWNER)
        // -----------------------------------------------------------
        [HttpGet("{orderId}")]
        public async Task<ActionResult<ApiResponse>> GetOrderById(Guid orderId)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(orderId);

                if (order == null)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Messages = "Order not found",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Order retrieved successfully",
                    Result = order,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order");

                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Messages = "Error retrieving order",
                    StatusCode = HttpStatusCode.InternalServerError
                });
            }
        }


        // -----------------------------------------------------------
        // GET ORDERS FOR A USER (USER CAN SEE HIS ORDERS ONLY)
        // -----------------------------------------------------------
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<ApiResponse>> GetOrdersByUser(Guid userId)
        {
            try
            {
                var orders = await _orderService.GetOrdersByUserIdAsync(userId);

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "User orders retrieved successfully",
                    Result = orders,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user orders");

                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Messages = "Error retrieving user orders",
                    StatusCode = HttpStatusCode.InternalServerError
                });
            }
        }
    }
}
