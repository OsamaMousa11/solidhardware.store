using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solidhardware.storeCore.DTO.WishListDTO
{
    public  class WishListResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public ICollection<WishlistItemResponse>? WishlistItems { get; set; }

    }
}
