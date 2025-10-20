
using Microsoft.AspNetCore.Identity;
using solidhardware.storeCore.DTO.AuthenticationDTO;
namespace solidhardware.storeCore.Domain.IdentityEntites
{
    public class ApplicationUser: IdentityUser<Guid>
    {
        public ICollection<RefreshToken>? RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
