using solidhardware.storeCore.Domain.Entites;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solidhardware.storeCore.DTO.OrderDTO
{
    public class OrderAddRequest
    {
        [Required]
        public Guid UserId { get; set; }
        public DateTime OrderDate { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Total amount must be positive")]
        public decimal TotalAmount { get; set; }
        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(100)]
        public string? Country { get; set; }
        [Phone]
        public string? Phone { get; set; }

        public ICollection<OrderItemAddRequest>? OrderItems { get; set; }
    }
}
