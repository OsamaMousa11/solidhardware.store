using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using solidhardware.storeCore.DTO;
using solidhardware.storeCore.DTO.BundleDTO;
using solidhardware.storeCore.ServiceContract;
using System.Net;

namespace solidhardware.storeApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BundleController : ControllerBase
    {
        private readonly IBundleService _bundleService;
        private readonly ILogger<BundleController> _logger;

        public BundleController(IBundleService bundleService, ILogger<BundleController> logger)
        {
            _bundleService = bundleService;
            _logger = logger;
        }

        // -------------------------------------------------------------
        // CREATE BUNDLE - ADMIN ONLY
        // -------------------------------------------------------------
        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        public async Task<ActionResult<ApiResponse>> CreateBundle([FromBody] BundleAddRequest request)
        {
            try
            {
                _logger.LogInformation("Creating new bundle");

                var bundle = await _bundleService.CreateAsync(request);

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Bundle created successfully",
                    Result = bundle,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating bundle");
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Messages = "An error occurred while creating bundle",
                    StatusCode = HttpStatusCode.InternalServerError
                });
            }
        }

        // -------------------------------------------------------------
        // GET BUNDLE BY ID
        // -------------------------------------------------------------
        [AllowAnonymous]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ApiResponse>> GetBundleById(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching bundle {BundleId}", id);

                var bundle = await _bundleService.GetAsync(b => b.Id == id);

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Bundle retrieved successfully",
                    Result = bundle,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bundle");
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Messages = "An error occurred while retrieving bundle",
                    StatusCode = HttpStatusCode.InternalServerError
                });
            }
        }

        // -------------------------------------------------------------
        // GET ALL BUNDLES
        // -------------------------------------------------------------
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<ApiResponse>> GetAllBundles(int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Fetching all bundles");

                var bundles = await _bundleService.GetAllAsync(pageIndex, pageSize);

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Bundles retrieved successfully",
                    Result = bundles,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching bundles");
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Messages = "An error occurred while retrieving bundles",
                    StatusCode = HttpStatusCode.InternalServerError
                });
            }
        }

        // -------------------------------------------------------------
        // UPDATE BUNDLE - ADMIN ONLY
        // -------------------------------------------------------------
        [Authorize(Roles = "ADMIN")]
        [HttpPut]
        public async Task<ActionResult<ApiResponse>> UpdateBundle([FromBody] BundleUpdateRequest request)
        {
            try
            {
                _logger.LogInformation("Updating bundle");

                var updatedBundle = await _bundleService.UpdateAsync(request);

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Bundle updated successfully",
                    Result = updatedBundle,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating bundle");
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Messages = "An error occurred while updating bundle",
                    StatusCode = HttpStatusCode.InternalServerError
                });
            }
        }

        // -------------------------------------------------------------
        // DELETE BUNDLE - ADMIN ONLY
        // -------------------------------------------------------------
        [Authorize(Roles = "ADMIN")]
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ApiResponse>> DeleteBundle(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting bundle {BundleId}", id);

                var result = await _bundleService.DeleteAsync(id);

                return Ok(new ApiResponse
                {
                    IsSuccess = result,
                    Messages = result ? "Bundle deleted successfully" : "Bundle not found",
                    Result = result,
                    StatusCode = result ? HttpStatusCode.OK : HttpStatusCode.NotFound
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting bundle");
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    Messages = "An error occurred while deleting bundle",
                    StatusCode = HttpStatusCode.InternalServerError
                });
            }
        }
    }
}
