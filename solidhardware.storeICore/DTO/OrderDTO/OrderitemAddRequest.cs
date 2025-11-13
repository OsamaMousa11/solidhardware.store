using solidhardware.storeCore.Domain.Entites;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solidhardware.storeCore.DTO.OrderDTO
{
    public class OrderItemAddRequest
    {

        [Required]
        public Guid ProductId { get; set; }
        [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100")]
        public int Quantity { get; set; } = 1;

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
        public decimal UnitPrice { get; set; }

    }
}
