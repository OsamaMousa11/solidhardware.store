using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solidhardware.storeCore.DTO.OrderDTO
{
    public class OrderUpdateRequest
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public string? City { get; set; }
        public string? Country { get; set; }
        public string? Phone { get; set; }

        public List<OrderItemUpdateRequest>? OrderItems { get; set; }
    }
}
