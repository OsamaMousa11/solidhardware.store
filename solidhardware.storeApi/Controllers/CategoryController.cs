using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using solidhardware.storeCore.DTO;
using solidhardware.storeCore.DTO.CategotyDTO;
using solidhardware.storeCore.ServiceContract;
using System.Net;

namespace solidhardware.storeApi.Controllers
{
    [Route("api/[controller]")]
  //  [Authorize(Roles = "ADMIN")]

    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;
        public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [HttpGet("GetAllCategory")]
        public async Task<ActionResult<ApiResponse>> GetAllCategory(int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                var Categries = await _categoryService.GetAllCategories(pageIndex, pageSize);
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Categories are fetched successfully",
                    Result = Categries,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetCategries method: An error occurred while fetched Categries");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while fetched Categories"
                });

            }
        }

        [HttpGet("GetCategoryById/{id:Guid}")]
        public async Task<ActionResult<ApiResponse>> GetCategoryById(Guid id)
        {
            try
            {
                var category = await _categoryService.GetCategory(x => x.Id == id);
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Categories are fetched successfully",
                    Result = category,
                    StatusCode = HttpStatusCode.OK
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetCategries method: An error occurred while fetched Category");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while fetched Category"
                });
            }
        }

        [HttpPost("CreateCategory")]
        public async Task<ActionResult<ApiResponse>> CreateCategory([FromBody] CategoryAddRequest categoryAddRequest)
        {
            try
            {
                var category = await _categoryService.CreateCategory(categoryAddRequest);
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Category  Created successfully",
                    Result = category,
                    StatusCode = HttpStatusCode.OK
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateCategory method: An error occurred while Create Category");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while Create Category"
                });
            }


        }
        [HttpDelete("DeleteCategory/{id:guid}")]

        public async Task<bool>DeleteAsync(Guid id)
        {
           try
            {
                _logger.LogInformation("DeleteCategory called for CategoryId: {CategoryId}", id);
                var result = await _categoryService.DeleteCategory(id);
                if(result)
                {
                    _logger.LogInformation("Category deleted successfully with ID: {CategoryId}", id);
                }
                else
                {
                          _logger.LogWarning("Category not found for deletion with ID: {CategoryId}", id);
                }
                return result;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting category with ID: {CategoryId}", id);
                throw;
            }

        }


        [HttpPut("UpdateCategory")]
        public async Task<ActionResult<ApiResponse>> UpdateCategory([FromBody] CategoryUpdateRequest categoryUpdateRequest)
        {
            try
            {
                var category = await _categoryService.UpdateCategory(categoryUpdateRequest);
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Category  Updated successfully",
                    Result = category,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateCategory method: An error occurred while Update Category");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while Update Category"
                });
            }
        }
    }
}
    

