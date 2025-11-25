using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using solidhardware.storeCore.DTO;
using solidhardware.storeCore.DTO.CategotyDTO;
using solidhardware.storeCore.ServiceContract;
using System.Net;

[Route("api/[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly ILogger<CategoryController> _logger;

    public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
    {
        _categoryService = categoryService;
        _logger = logger;
    }


    // --------------------------------------------------------------------
    // PUBLIC: GET ALL
    // --------------------------------------------------------------------
    [AllowAnonymous]
    [HttpGet("GetAllCategory")]
    public async Task<ActionResult<ApiResponse>> GetAllCategory(int pageIndex = 1, int pageSize = 10)
    {
        try
        {
            var categories = await _categoryService.GetAllCategories(pageIndex, pageSize);

            return Ok(new ApiResponse
            {
                IsSuccess = true,
                Messages = "Categories loaded",
                Result = categories,
                StatusCode = HttpStatusCode.OK
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching categories");
            return StatusCode(500, new ApiResponse
            {
                IsSuccess = false,
                Messages = "Error fetching categories",
                StatusCode = HttpStatusCode.InternalServerError
            });
        }
    }


    // --------------------------------------------------------------------
    // PUBLIC: GET BY ID
    // --------------------------------------------------------------------
    [AllowAnonymous]
    [HttpGet("GetCategoryById/{id:Guid}")]
    public async Task<ActionResult<ApiResponse>> GetCategoryById(Guid id)
    {
        try
        {
            var category = await _categoryService.GetCategory(x => x.Id == id);

            return Ok(new ApiResponse
            {
                IsSuccess = true,
                Messages = "Category loaded",
                Result = category,
                StatusCode = HttpStatusCode.OK
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching category");
            return StatusCode(500, new ApiResponse
            {
                IsSuccess = false,
                Messages = "Error fetching category",
                StatusCode = HttpStatusCode.InternalServerError
            });
        }
    }


    // --------------------------------------------------------------------
    // ADMIN ONLY: CREATE
    // --------------------------------------------------------------------
    [Authorize(Roles = "ADMIN")]
    [HttpPost("CreateCategory")]
    public async Task<ActionResult<ApiResponse>> CreateCategory([FromBody] CategoryAddRequest request)
    {
        try
        {
            var category = await _categoryService.CreateCategory(request);

            return Ok(new ApiResponse
            {
                IsSuccess = true,
                Messages = "Category created",
                Result = category,
                StatusCode = HttpStatusCode.OK
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category");

            return StatusCode(500, new ApiResponse
            {
                IsSuccess = false,
                Messages = "Error creating category",
                StatusCode = HttpStatusCode.InternalServerError
            });
        }
    }


    // --------------------------------------------------------------------
    // ADMIN ONLY: DELETE
    // --------------------------------------------------------------------
    [Authorize(Roles = "ADMIN")]
    [HttpDelete("DeleteCategory/{id:guid}")]
    public async Task<ActionResult<ApiResponse>> DeleteAsync(Guid id)
    {
        try
        {
            var result = await _categoryService.DeleteCategory(id);

            return Ok(new ApiResponse
            {
                IsSuccess = result,
                Messages = result ? "Deleted successfully" : "Category not found",
                StatusCode = result ? HttpStatusCode.OK : HttpStatusCode.NotFound
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category");

            return StatusCode(500, new ApiResponse
            {
                IsSuccess = false,
                Messages = "Error deleting category",
                StatusCode = HttpStatusCode.InternalServerError
            });
        }
    }


    // --------------------------------------------------------------------
    // ADMIN ONLY: UPDATE
    // --------------------------------------------------------------------
    [Authorize(Roles = "ADMIN")]
    [HttpPut("UpdateCategory")]
    public async Task<ActionResult<ApiResponse>> UpdateCategory([FromBody] CategoryUpdateRequest request)
    {
        try
        {
            var category = await _categoryService.UpdateCategory(request);

            return Ok(new ApiResponse
            {
                IsSuccess = true,
                Messages = "Category updated",
                Result = category,
                StatusCode = HttpStatusCode.OK
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category");

            return StatusCode(500, new ApiResponse
            {
                IsSuccess = false,
                Messages = "Error updating category",
                StatusCode = HttpStatusCode.InternalServerError
            });
        }
    }
}
