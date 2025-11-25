using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using solidhardware.storeCore.DTO;
using solidhardware.storeCore.DTO.ProductDTO;
using solidhardware.storeCore.ServiceContract;
using System.Net;

namespace solidhardware.storeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }


        // -----------------------------------------------------------
        // CREATE PRODUCT (ADMIN ONLY)
        // -----------------------------------------------------------
        [Authorize(Roles = "ADMIN")]
        [HttpPost("CreateProduct")]
        public async Task<ActionResult<ApiResponse>> CreateProduct([FromBody] ProductAddRequest productAddRequest)
        {
            try
            {
                var product = await _productService.CreateProductAsync(productAddRequest);

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Product created successfully",
                    Result = product,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Messages = "Error creating product",
                    StatusCode = HttpStatusCode.InternalServerError
                });
            }
        }


        // -----------------------------------------------------------
        // GET PRODUCT BY ID (PUBLIC)
        // -----------------------------------------------------------
        [AllowAnonymous]
        [HttpGet("GetProductById/{id:guid}")]
        public async Task<ActionResult<ApiResponse>> GetProductById(Guid id)
        {
            try
            {
                var product = await _productService.GetProduct(p => p.Id == id);

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Product retrieved successfully",
                    Result = product,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product");
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Messages = "Error retrieving product",
                    StatusCode = HttpStatusCode.InternalServerError
                });
            }
        }


        // -----------------------------------------------------------
        // GET ALL PRODUCTS (PUBLIC)
        // -----------------------------------------------------------
        [AllowAnonymous]
        [HttpGet("GetAllProducts")]
        public async Task<ActionResult<ApiResponse>> GetAllProducts(int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                var products = await _productService.GetAllProducts(pageIndex, pageSize);

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Products retrieved successfully",
                    Result = products,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving products");
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Messages = "Error retrieving products",
                    StatusCode = HttpStatusCode.InternalServerError
                });
            }
        }


        // -----------------------------------------------------------
        // DELETE PRODUCT (ADMIN ONLY)
        // -----------------------------------------------------------
        [Authorize(Roles = "ADMIN")]
        [HttpDelete("DeleteProduct/{id:guid}")]
        public async Task<ActionResult<ApiResponse>> DeleteProduct(Guid id)
        {
            try
            {
                var result = await _productService.DeleteProductAsync(id);

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = result ? "Product deleted successfully" : "Product not found",
                    Result = result,
                    StatusCode = result ? HttpStatusCode.OK : HttpStatusCode.NotFound
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product");
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Messages = "Error deleting product",
                    StatusCode = HttpStatusCode.InternalServerError
                });
            }
        }


        // -----------------------------------------------------------
        // GET BY CATEGORY (PUBLIC)
        // -----------------------------------------------------------
        [AllowAnonymous]
        [HttpGet("GetProductsByCategory/{categoryId:guid}")]
        public async Task<ActionResult<ApiResponse>> GetProductsByCategory(Guid categoryId)
        {
            try
            {
                var products = await _productService.GetProductsByCategory(categoryId);

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Products retrieved successfully",
                    Result = products,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving products by category");
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Messages = "Error retrieving products by category",
                    StatusCode = HttpStatusCode.InternalServerError
                });
            }
        }


        // -----------------------------------------------------------
        // SEARCH (PUBLIC)
        // -----------------------------------------------------------
        [AllowAnonymous]
        [HttpGet("SearchProducts")]
        public async Task<ActionResult<ApiResponse>> SearchProducts(string searchTerm)
        {
            try
            {
                var products = await _productService.SearchProducts(searchTerm);

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Products retrieved successfully",
                    Result = products,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching products");
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Messages = "Error searching products",
                    StatusCode = HttpStatusCode.InternalServerError
                });
            }
        }


        // -----------------------------------------------------------
        // UPDATE PRODUCT (ADMIN ONLY)
        // -----------------------------------------------------------
        [Authorize(Roles = "ADMIN")]
        [HttpPut("UpdateProduct")]
        public async Task<ActionResult<ApiResponse>> UpdateProduct([FromBody] ProductUpdateRequest productUpdateRequest)
        {
            try
            {
                var updatedProduct = await _productService.UpdateProduct(productUpdateRequest);

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Product updated successfully",
                    Result = updatedProduct,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product");
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Messages = "Error updating product",
                    StatusCode = HttpStatusCode.InternalServerError
                });
            }
        }
    }
}
