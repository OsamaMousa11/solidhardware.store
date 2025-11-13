using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solidhardware.storeCore.DTO.OrderDTO
{
    public class OrderItemUpdateRequest
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100")]
        public int Quantity { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than zero")]
        public decimal UnitPrice { get; set; }

        public decimal SubTotal => Quantity * UnitPrice;
    }
}
