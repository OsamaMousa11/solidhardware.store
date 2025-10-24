
using Microsoft.AspNetCore.Identity;
using solidhardware.storeCore.Domain.Entites;
using solidhardware.storeCore.DTO.AuthenticationDTO;
namespace solidhardware.storeCore.Domain.IdentityEntites
{
    public class ApplicationUser: IdentityUser<Guid>
    { 
        public ICollection<Review>? Reviews { get; set; }
        public ICollection<Order>? Orders { get; set; } 
        public ICollection<Wishlist>? Wishlists { get; set; }
        public Cart? Cart { get; set; }

        public ICollection<RefreshToken>? RefreshTokens { get; set; } 
    }
}
