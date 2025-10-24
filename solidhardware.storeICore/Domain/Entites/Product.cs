using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solidhardware.storeCore.Domain.Entites
{
    public class Product
    {
        public  Guid Id {get; set;}
        public string Name {get; set;}
        public string Description {get; set;}

        public decimal Price {get; set;}
        public int Stock_quantity {get; set;}

        public string ImageUrl {get; set;}

        public string Brand {get; set;}
        public Guid CategoryId  {get; set;}
        public Category Category {get; set; }

        public ProductSpecialProperty ProductSpecialProperty { get; set; }
        public ICollection<BundleItem> BundleItems {get; set; }
        public ICollection<OrderItem> OrderItems {get; set; }
        public ICollection<Cartitem> CartItems { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<WishlistItem> WishlistItems { get; set; }


    }
}
