using solidhardware.storeCore.Domain.IdentityEntites;
using solidhardware.storeCore.DTO.AuthenticationDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solidhardware.storeCore.ServiceContract
{
    public interface IAuthenticationServices
    {

        // ====== Authentication ======
        Task<AuthenticationResponse> RegisterAsync(RegisterDTO registerDTO);
        Task<AuthenticationResponse> LoginAsync(LoginDTO loginDTO);
        Task<AuthenticationResponse> RefreshTokenAsync(string token);
        Task<bool> RevokeTokenAsync(string token);

        // ====== Roles ======
        Task<string> AddRoleToUserAsync(AddRoleDTO model);
        Task<IEnumerable<ApplicationRole>> GetAllRolesAsync();
        Task<string> DeleteRoleAsync(string roleName);

        // ====== Users ======
        Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
        Task<ApplicationUser?> GetUserByIdAsync(string id);
        Task<string> UpdateUserAsync(UpdateUserDTO dto);
        Task<string> DeleteUserAsync(string id);
    }
}
