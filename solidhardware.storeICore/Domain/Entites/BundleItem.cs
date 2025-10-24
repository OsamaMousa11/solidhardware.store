using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solidhardware.storeCore.Domain.Entites
{
    public class BundleItem
    {
        public Guid Id { get; set; }

       
        public Guid BundleId { get; set; }
        public Bundle Bundle { get; set; }

      
        public Guid ProductId { get; set; }
        public Product Product { get; set; }

       
        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }
    }
}
