using solidhardware.storeCore.Domain.Entites;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solidhardware.storeCore.DTO.ProductDTO
{
    public class ProductSpecialPropertyUpdateRequest
    {
        [Required(ErrorMessage = "Id is required")]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "ProductId is required")]
        public Guid ProductId { get; set; }


        [Required(ErrorMessage = "Key is required")]
        [MaxLength(200)]
        public string Key { get; set; }

        [Required(ErrorMessage = "Value is required")]
        [MaxLength(200)]
        public string Value { get; set; }

        [Range(0, int.MaxValue)]
        public int Size { get; set; }

        [MaxLength(20)]
        public string? Unit { get; set; }

        [Range(0, 1000)]
        public int DisplayOrder { get; set; }
    }
}
