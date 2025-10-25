using Microsoft.AspNetCore.Http;
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
        private readonly IAuthenticationServices _authenticationServices;
        private readonly IMailingService _mailingService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(
            IAuthenticationServices authenticationServices,
            IMailingService mailingService,
            UserManager<ApplicationUser> userManager)
        {
            _authenticationServices = authenticationServices;
            _mailingService = mailingService;
            _userManager = userManager;
        }

        // ============================================================
        // 🔹 SECTION 1 : Authentication (Register / Login)
        // ============================================================

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authenticationServices.RegisterAsync(dto);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            SetRefreshToken(result.RefreshToken, result.RefreshTokenExpiration);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authenticationServices.LoginAsync(dto);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            SetRefreshToken(result.RefreshToken, result.RefreshTokenExpiration);
            return Ok(result);
        }

        // ============================================================
        // 🔹 SECTION 2 : Refresh / Revoke Token
        // ============================================================

        [HttpGet("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var token = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(token))
                return BadRequest("Refresh token not found");

            var result = await _authenticationServices.RefreshTokenAsync(token);
            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            SetRefreshToken(result.RefreshToken, result.RefreshTokenExpiration);
            return Ok(result);
        }

        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken([FromBody] RevokTokenDTO model)
        {
            var token = model.Token ?? Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(token))
                return BadRequest("Token is required");

            var result = await _authenticationServices.RevokeTokenAsync(token);
            if (!result)
                return BadRequest("Invalid or inactive token");

            return Ok(new { message = "Token revoked successfully" });
        }

        // ============================================================
        // 🔹 SECTION 3 : Password Reset (Forgot / Reset)
        // ============================================================

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO dto)
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

            var callback = QueryHelpers.AddQueryString(dto.ClientUri!, param);
            await _mailingService.SendMessageAsync(user.Email!, "Reset Password", callback, null);

            return Ok(new { message = "Password reset link sent" });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO dto)
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

        // ============================================================
        // 🔹 SECTION 4 : Roles Management
        // ============================================================

        [HttpPost("add-role")]
        public async Task<IActionResult> AddRole([FromBody] AddRoleDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authenticationServices.AddRoleToUserAsync(dto);
            if (!string.IsNullOrEmpty(result))
                return BadRequest(result);

            return Ok(new { message = "Role added successfully" });
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _authenticationServices.GetAllRolesAsync();
            return Ok(roles);
        }

        [HttpDelete("roles/{roleName}")]
        public async Task<IActionResult> DeleteRole(string roleName)
        {
            var result = await _authenticationServices.DeleteRoleAsync(roleName);
            return Ok(new { message = result });
        }

        // ============================================================
        // 🔹 SECTION 5 : User Management
        // ============================================================

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _authenticationServices.GetAllUsersAsync();
            var result = users.Select(u => new
            {
                u.Id,
                u.UserName,
                u.Email,
                u.PhoneNumber
            });

            return Ok(result);
        }

        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _authenticationServices.GetUserByIdAsync(id);
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

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUser([FromRoute] string id, [FromBody] UpdateUserDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            dto.Id = id;
            var result = await _authenticationServices.UpdateUserAsync(dto);

            if (result.Contains("not found"))
                return NotFound(new { message = result });

            return Ok(new { message = result });
        }


        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _authenticationServices.DeleteUserAsync(id);
            if (result.Contains("not found"))
                return NotFound(result);

            return Ok(new { message = result });
        }

        // ============================================================
        // 🔹 SECTION 6 : Helpers
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
