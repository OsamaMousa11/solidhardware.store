using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solidhardware.storeCore.DTO.OrderDTO
{
    public class OrderItemAddRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; } = 1;
    
    }
}
