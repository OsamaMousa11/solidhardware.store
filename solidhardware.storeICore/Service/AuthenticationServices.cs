using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using solidhardware.storeCore.Domain.IdentityEntites;
using solidhardware.storeCore.DTO;
using solidhardware.storeCore.DTO.AuthenticationDTO;
using solidhardware.storeCore.Enumuration;
using solidhardware.storeCore.ServiceContract;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace solidhardware.storeCore.Service
{
    public class AuthenticationServices : IAuthenticationServices
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly JwtDTO _jwt;

        public AuthenticationServices(UserManager<ApplicationUser> userManager,
                                      RoleManager<ApplicationRole> roleManager,
                                      IOptions<JwtDTO> jwt)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwt = jwt.Value;
        }

        // ========================= Authentication ============================= //

        public async Task<AuthenticationResponse> RegisterAsync(RegisterDTO registerDTO)
        {
            if (await _userManager.FindByEmailAsync(registerDTO.Email) != null)
                return new AuthenticationResponse { Message = "Email is already registered!" };

            if (await _userManager.FindByNameAsync(registerDTO.UserName) != null)
                return new AuthenticationResponse { Message = "Username is already registered!" };

            var user = new ApplicationUser
            {
                UserName = registerDTO.UserName,
                Email = registerDTO.Email,
            };

            var result = await _userManager.CreateAsync(user, registerDTO.Password);
            if (!result.Succeeded)
                return CreateErrorResponse(result.Errors);

            var roleName = registerDTO.RolesOption == RolesOption.ADMIN ? "ADMIN" : "USER";
            var addRoleResult = await AddRoleToUserAsync(new AddRoleDTO { RoleName = roleName, UserID = user.Id });

            if (!string.IsNullOrEmpty(addRoleResult))
                return new AuthenticationResponse { Message = addRoleResult };

            var authenticationUser = await GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();

            authenticationUser.RefreshToken = newRefreshToken.Token;
            authenticationUser.RefreshTokenExpiration = newRefreshToken.ExpiredOn;

            user.RefreshTokens.Add(newRefreshToken);
            await _userManager.UpdateAsync(user);

            return authenticationUser;
        }

        public async Task<AuthenticationResponse> LoginAsync(LoginDTO loginDTO)
        {
            var user = await _userManager.FindByNameAsync(loginDTO.UserName);

            if (user is null || !await _userManager.CheckPasswordAsync(user, loginDTO.Password))
                return new AuthenticationResponse { Message = "Email or Password is incorrect!" };

            var authenticationUser = await GenerateJwtToken(user);

            if (user.RefreshTokens.Any(x => x.IsActive))
            {
                var activeRefreshToken = user.RefreshTokens.First(x => x.IsActive);
                authenticationUser.RefreshToken = activeRefreshToken.Token;
                authenticationUser.RefreshTokenExpiration = activeRefreshToken.ExpiredOn;
            }
            else
            {
                var newRefreshToken = GenerateRefreshToken();
                authenticationUser.RefreshToken = newRefreshToken.Token;
                authenticationUser.RefreshTokenExpiration = newRefreshToken.ExpiredOn;
                user.RefreshTokens.Add(newRefreshToken);
                await _userManager.UpdateAsync(user);
            }

            return authenticationUser;
        }

        public async Task<AuthenticationResponse> RefreshTokenAsync(string token)
        {
            var user = _userManager.Users.SingleOrDefault(x => x.RefreshTokens.Any(rt => rt.Token == token));
            if (user == null)
                return new AuthenticationResponse { Message = "Invalid token" };

            var refreshToken = user.RefreshTokens.Single(rt => rt.Token == token);
            if (!refreshToken.IsActive)
                return new AuthenticationResponse { Message = "Inactive token" };

            refreshToken.RevokedOn = DateTime.UtcNow;
            var newRefreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            await _userManager.UpdateAsync(user);

            var authenticationUser = await GenerateJwtToken(user);
            authenticationUser.RefreshToken = newRefreshToken.Token;
            authenticationUser.RefreshTokenExpiration = newRefreshToken.ExpiredOn;

            return authenticationUser;
        }

        public async Task<bool> RevokeTokenAsync(string token)
        {
            var user = _userManager.Users.SingleOrDefault(x => x.RefreshTokens.Any(rt => rt.Token == token));
            if (user == null)
                return false;

            var refreshToken = user.RefreshTokens.Single(rt => rt.Token == token);
            if (!refreshToken.IsActive)
                return false;

            refreshToken.RevokedOn = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
            return true;
        }

        private AuthenticationResponse CreateErrorResponse(IEnumerable<IdentityError> errors)
        {
            var errorMessages = string.Join(", ", errors.Select(e => e.Description));
            return new AuthenticationResponse { Message = errorMessages };
        }

        private RefreshToken GenerateRefreshToken()
        {
            byte[] bytes = new byte[64];
            RandomNumberGenerator.Fill(bytes);
            return new RefreshToken()
            {
                CreatedOn = DateTime.UtcNow,
                ExpiredOn = DateTime.UtcNow.AddDays(10),
                Token = Convert.ToBase64String(bytes)
            };
        }

        private async Task<AuthenticationResponse> GenerateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            var roleClaims = roles.Select(role => new Claim("roles", role)).ToList();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id.ToString())
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(_jwt.DurationInDays),
                signingCredentials: signingCredentials);

            return new AuthenticationResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Email = user.Email,
                Username = user.UserName,
                Roles = roles.ToList(),
                Message = "Success Operation",
                RefreshTokenExpiration = DateTime.UtcNow.AddDays(_jwt.DurationInDays),
                IsAuthenticated = true
            };
        }

        // ========================= Role Management ============================= //

        public async Task<string> AddRoleToUserAsync(AddRoleDTO model)
        {
            var role = await _roleManager.FindByNameAsync(model.RoleName);
            if (role == null)
            {
                var createRoleResult = await _roleManager.CreateAsync(new ApplicationRole { Name = model.RoleName });
                if (!createRoleResult.Succeeded)
                    return string.Join(", ", createRoleResult.Errors.Select(e => e.Description));
            }

            var user = await _userManager.FindByIdAsync(model.UserID.ToString());
            if (user == null)
                return "User not found.";

            if (await _userManager.IsInRoleAsync(user, model.RoleName))
                return "User is already assigned to this role.";

            var addRoleResult = await _userManager.AddToRoleAsync(user, model.RoleName);
            return addRoleResult.Succeeded ? null : string.Join(", ", addRoleResult.Errors.Select(e => e.Description));
        }

        public async Task<IEnumerable<ApplicationRole>> GetAllRolesAsync()
        {
            return _roleManager.Roles.ToList();
        }

        public async Task<string> DeleteRoleAsync(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
                return "Role not found";

            var result = await _roleManager.DeleteAsync(role);
            return result.Succeeded ? "Role deleted successfully"
                                    : string.Join(", ", result.Errors.Select(e => e.Description));
        }

        // ========================= User Management ============================= //

        public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
        {
            return _userManager.Users.ToList();
        }

        public async Task<ApplicationUser?> GetUserByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<string> UpdateUserAsync(UpdateUserDTO dto)
        {
            var user = await _userManager.FindByIdAsync(dto.Id);
            if (user == null)
                return "User not found!";


            if (!string.IsNullOrEmpty(dto.Email) && dto.Email != user.Email)
            {
                if (!new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(dto.Email))
                    return "Invalid email format!";

                var existingEmailUser = await _userManager.FindByEmailAsync(dto.Email);
                if (existingEmailUser != null && existingEmailUser.Id != user.Id)
                    return "Email is already registered!";
            }


            if (!string.IsNullOrEmpty(dto.UserName) && dto.UserName != user.UserName)
            {
                var existingUserName = await _userManager.FindByNameAsync(dto.UserName);
                if (existingUserName != null && existingUserName.Id != user.Id)
                    return "Username is already registered!";
            }


            if (!string.IsNullOrEmpty(dto.NewPassword))
            {
                var passwordValidator = new PasswordValidator<ApplicationUser>();
                var passwordValidation = await passwordValidator.ValidateAsync(_userManager, user, dto.NewPassword);

                if (!passwordValidation.Succeeded)
                    return string.Join(", ", passwordValidation.Errors.Select(e => e.Description));


                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetResult = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);
                if (!resetResult.Succeeded)
                    return string.Join(", ", resetResult.Errors.Select(e => e.Description));
            }


            user.Email = dto.Email ?? user.Email;
            user.UserName = dto.UserName ?? user.UserName;
            user.PhoneNumber = dto.PhoneNumber ?? user.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return string.Join(", ", result.Errors.Select(e => e.Description));

            return "User updated successfully";
        }


        public async Task<string> DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return "User not found";

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded ? "User deleted successfully"
                                    : string.Join(", ", result.Errors.Select(e => e.Description));
        }
    }
}
