using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using solidhardware.storeCore.Domain.Entites;
using solidhardware.storeCore.DTO;
using solidhardware.storeCore.DTO.BundleDTO;
using solidhardware.storeCore.Service;
using solidhardware.storeCore.ServiceContract;
using System.Net;

namespace solidhardware.storeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BundleController : ControllerBase
    {
        private readonly IBundleService _bundleService;
        private readonly ILogger<BundleController> _logger;
        public BundleController(IBundleService bundleService, ILogger<BundleController> logger)
        {
            _bundleService = bundleService;
            _logger = logger;
        }

        [HttpPost("CreateBundle")]
        public async Task<ActionResult<ApiResponse>> CreateBundle([FromBody] BundleAddRequest bundleAddRequest)
        {
            _logger.LogInformation("CreateBundle");

            try
            {
                var createdBundle = await _bundleService.CreateAsync(bundleAddRequest);
                var response = new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Bundle created successfully",
                    Result = createdBundle,
                    StatusCode = System.Net.HttpStatusCode.OK
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateProduct method: An error occurred while creating Bundle");
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while creating Bundle"
                });
            }
        }

        [HttpGet("GetBundleById/{id:guid}")]
        public async Task<ActionResult<ApiResponse>> GetBundleById(Guid? id)
        {
            _logger.LogInformation("GetBundleById");
            try
            {
                var bundle = await _bundleService.GetAsync(p => p.Id == id);
                var response = new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Bundle retrieved successfully",
                    Result = bundle,
                    StatusCode = System.Net.HttpStatusCode.OK
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetBundleById method: An error occurred while retrieving Bundle");
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while retrieving Bundle"
                });
            }
        }
        [HttpGet("GetAllBundles")]
        public async Task<ActionResult<ApiResponse>> GetAllBundles(int pageIndex = 1, int pageSize = 10)
        {
            _logger.LogInformation("GetAllBundles");
            try
            {
                var bundles = await _bundleService.GetAllAsync(pageIndex, pageSize);
                var response = new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Bundles retrieved successfully",
                    Result = bundles,
                    StatusCode = System.Net.HttpStatusCode.OK
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllBundles method: An error occurred while retrieving Bundles");
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while retrieving Bundles"
                });
            }
        }

        [HttpDelete("DeleteBundle/{id:guid}")]
        public async Task<ActionResult<ApiResponse>> DeleteBundle(Guid id)
        {
            try
            {
                var result = await _bundleService.DeleteAsync(id);
                return Ok(new ApiResponse
                {
                    IsSuccess = result,
                    Messages = result ? "Bundle deleted successfully" : "Bundle not found",
                    Result = null,
                    StatusCode = result ? HttpStatusCode.OK : HttpStatusCode.NotFound
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteBundle method: An error occurred while deleting Bundle");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while deleting Bundle"
                });
            }
        }

        [HttpPut("UpdateBundle")]
        public async Task<ActionResult<ApiResponse>> UpdateBundle([FromBody] BundleUpdateRequest bundleUpdateRequest)
        {
            _logger.LogInformation("UpdateBundle");
            try
            {
                var updatedBundle = await _bundleService.UpdateAsync(bundleUpdateRequest);
                var response = new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Bundle updated successfully",
                    Result = updatedBundle,
                    StatusCode = HttpStatusCode.OK
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateBundle method: An error occurred while updating Bundle");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while updating Bundle"
                });
            }
        }
    }
}
