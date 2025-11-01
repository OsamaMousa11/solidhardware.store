using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solidhardware.storeCore.DTO.WishListDTO
{
    public class WishlistUpdateRequest
    {
        [Required(ErrorMessage = "Wishlist Id is required")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "UserId is required")]
        public Guid UserId { get; set; }

        public ICollection<WishlistItemUpdateRequest>? WishlistItems { get; set; }
    }
}
