using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using solidhardware.storeCore.Domain.IdentityEntites;
using solidhardware.storeCore.DTO.AuthenticationDTO;
using solidhardware.storeCore.ServiceContract;

namespace solidhardware.storeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthenticationServices _authService;
        private readonly IMailingService _mailService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            IAuthenticationServices authService,
            IMailingService mailService,
            UserManager<ApplicationUser> userManager,
            ILogger<AccountController> logger)
        {
            _authService = authService;
            _mailService = mailService;
            _userManager = userManager;
            _logger = logger;
        }

        // ============================================================
        // REGISTER
        // ============================================================
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _authService.RegisterAsync(dto);

                if (!result.IsAuthenticated)
                    return BadRequest(result.Message);

                SetRefreshToken(result.RefreshToken, result.RefreshTokenExpiration);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Register");
                return StatusCode(500, "Internal Server Error");
            }
        }

        // ============================================================
        // LOGIN
        // ============================================================
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _authService.LoginAsync(dto);

                if (!result.IsAuthenticated)
                    return BadRequest(result.Message);

                SetRefreshToken(result.RefreshToken, result.RefreshTokenExpiration);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Login");
                return StatusCode(500, "Internal Server Error");
            }
        }

        // ============================================================
        // REFRESH TOKEN
        // ============================================================
        [HttpGet("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            try
            {
                var token = Request.Cookies["refreshToken"];
                if (string.IsNullOrEmpty(token))
                    return BadRequest("Refresh token not found");

                var result = await _authService.RefreshTokenAsync(token);

                if (!result.IsAuthenticated)
                    return BadRequest(result.Message);

                SetRefreshToken(result.RefreshToken, result.RefreshTokenExpiration);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RefreshToken");
                return StatusCode(500, "Internal Server Error");
            }
        }

        // ============================================================
        // REVOKE TOKEN
        // ============================================================
        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken([FromBody] RevokTokenDTO dto)
        {
            try
            {
                var token = dto.Token ?? Request.Cookies["refreshToken"];
                if (string.IsNullOrEmpty(token))
                    return BadRequest("Token is required");

                var result = await _authService.RevokeTokenAsync(token);

                if (!result)
                    return BadRequest("Invalid or inactive token");

                return Ok(new { message = "Token revoked successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RevokeToken");
                return StatusCode(500, "Internal Server Error");
            }
        }

        // ============================================================
        // FORGOT PASSWORD
        // ============================================================
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _userManager.FindByEmailAsync(dto.Email!);
                if (user == null)
                    return BadRequest("Invalid email");

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                var param = new Dictionary<string, string?>
                {
                    {"token", token},
                    {"email", dto.Email}
                };

                var callbackUrl = QueryHelpers.AddQueryString(dto.ClientUri!, param);

                await _mailService.SendMessageAsync(user.Email!, "Reset Password", callbackUrl, null);

                return Ok(new { message = "Password reset link sent" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ForgotPassword");
                return StatusCode(500, "Internal Server Error");
            }
        }

        // ============================================================
        // RESET PASSWORD
        // ============================================================
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _userManager.FindByEmailAsync(dto.Email!);
                if (user == null)
                    return BadRequest("Invalid request");

                var result = await _userManager.ResetPasswordAsync(user, dto.Token!, dto.Password!);

                if (!result.Succeeded)
                    return BadRequest(result.Errors.Select(e => e.Description));

                return Ok(new { message = "Password reset successful" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ResetPassword");
                return StatusCode(500, "Internal Server Error");
            }
        }

        // ============================================================
        // ROLES
        // ============================================================
        [Authorize(Roles = "ADMIN")]
        [HttpPost("add-role")]
        public async Task<IActionResult> AddRole([FromBody] AddRoleDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _authService.AddRoleToUserAsync(dto);

                if (!string.IsNullOrEmpty(result))
                    return BadRequest(result);

                return Ok(new { message = "Role added successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddRole");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
            try
            {
                var roles = await _authService.GetAllRolesAsync();
                return Ok(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetRoles");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [Authorize(Roles = "ADMIN")]
        [HttpDelete("roles/{roleName}")]
        public async Task<IActionResult> DeleteRole(string roleName)
        {
            try
            {
                var result = await _authService.DeleteRoleAsync(roleName);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteRole");
                return StatusCode(500, "Internal Server Error");
            }
        }

        // ============================================================
        // USERS MANAGEMENT
        // ============================================================
        [Authorize(Roles = "ADMIN")]
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = await _authService.GetAllUsersAsync();

                return Ok(users.Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.Email,
                    u.PhoneNumber
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetUsers");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            try
            {
                var user = await _authService.GetUserByIdAsync(id);

                if (user == null)
                    return NotFound("User not found");

                return Ok(new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                    user.PhoneNumber
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetUserById");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                dto.Id = id;

                var result = await _authService.UpdateUserAsync(dto);

                if (result.Contains("not found"))
                    return NotFound(new { message = result });

                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateUser");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [Authorize(Roles = "ADMIN")]
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var result = await _authService.DeleteUserAsync(id);

                if (result.Contains("not found"))
                    return NotFound(result);

                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteUser");
                return StatusCode(500, "Internal Server Error");
            }
        }

        // ============================================================
        // HELPER: SET REFRESH TOKEN
        // ============================================================
        private void SetRefreshToken(string refreshToken, DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = expires
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    }
}
