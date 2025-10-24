using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solidhardware.storeCore.Domain.Entites
{
    public class Bundle
    {  

        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public Decimal Price { get; set; }

        public string ImageUrl { get; set; }

        public ICollection<BundleItem> BundleItems { get; set; }
    }
}
