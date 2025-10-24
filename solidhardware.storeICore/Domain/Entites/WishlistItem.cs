using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solidhardware.storeCore.Domain.Entites
{
     public class WishlistItem
    {
        public int Id { get; set; }

        public Guid WishlistId { get; set; }
        public Wishlist Wishlist { get; set; }

        public Guid ProductId { get; set; }
        public Product Product { get; set; }

    }
}
