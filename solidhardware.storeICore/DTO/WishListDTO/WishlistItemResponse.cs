using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solidhardware.storeCore.DTO.WishListDTO
{
    public  class WishlistItemResponse
    {
        public Guid Id { get; set; }
        public Guid WishlistId { get; set; }
        public Guid ProductId { get; set; }
    }
}
