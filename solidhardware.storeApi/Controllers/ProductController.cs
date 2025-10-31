using Microsoft.AspNetCore.Http;
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
            _logger = logger;
            _productService = productService;
        }
        [HttpPost("CreateProduct")]
        public async Task<ActionResult<ApiResponse>> CreateProduct([FromBody] ProductAddRequest productAddRequest)
        {

            try
            {
                var Product = await _productService.CreateProductAsync(productAddRequest);
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Product created successfully",
                    Result = Product,
                    StatusCode = System.Net.HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateProduct method: An error occurred while creating product");
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while creating product"
                });
            }
        }

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
                _logger.LogError(ex, "GetProductById method: An error occurred while retrieving product");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while retrieving product"
                });
            }
        }
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
                _logger.LogError(ex, "GetAllProducts method: An error occurred while retrieving products");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while retrieving products"
                });
            }
        }

        [HttpDelete("DeleteProduct/{id:guid}")]
        public async Task<ActionResult<ApiResponse>> DeleteProduct(Guid id)
        {
            try
            {
                var result = await _productService.DeleteProductAsync(id);
                return Ok(new ApiResponse
                {
                    IsSuccess = result,
                    Messages = result ? "Product deleted successfully" : "Product not found",
                    Result = null,
                    StatusCode = result ? HttpStatusCode.OK : HttpStatusCode.NotFound
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteProduct method: An error occurred while deleting product");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while deleting product"
                });
            }
        }
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
                _logger.LogError(ex, "GetProductsByCategory method: An error occurred while retrieving products by category");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while retrieving products by category"
                });
            }
        }
        [HttpGet("SearchProducts")]
        public async Task <ActionResult<ApiResponse>> SearchProducts(string searchTerm)
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
                _logger.LogError(ex, "SearchProducts method: An error occurred while searching products");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while searching products"
                });
            }
        }

        [HttpPut("UpdateProduct")]
        public async Task<ActionResult<ApiResponse>> UpdateProduct( [FromBody] ProductUpdateRequest productUpdateRequest)
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
                _logger.LogError(ex, "UpdateProduct method: An error occurred while updating product");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while updating product"
                });
            }
        }
    }
}
