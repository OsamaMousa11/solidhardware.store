using solidhardware.storeCore.Domain.Entites;
using solidhardware.storeCore.Domain.IdentityEntites;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solidhardware.storeCore.DTO.WishListDTO
{
    public class WitshListAddRequest
    {
        [Required(ErrorMessage = "UserId  is Required ")]
        public Guid UserId { get; set; }

        public ICollection<WishlistItemAddRequest>? WishlistItemAddRequest { get; set; }
    } 
}
