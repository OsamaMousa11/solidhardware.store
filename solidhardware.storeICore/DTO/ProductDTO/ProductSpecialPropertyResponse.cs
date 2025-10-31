using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solidhardware.storeCore.DTO.ProductDTO
{
     public  class ProductSpecialPropertyResponse
    {

     
        public Guid? Id { get; set; }

        
        public string Key { get; set; }
        public string Value { get; set; }

        public int Size { get; set; }

        public string? Unit { get; set; }

        public int DisplayOrder { get; set; }

    }
}
