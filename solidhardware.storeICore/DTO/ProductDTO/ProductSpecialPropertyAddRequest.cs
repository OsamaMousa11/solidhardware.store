using System;
using System.ComponentModel.DataAnnotations;

namespace solidhardware.storeCore.DTO.ProductDTO
{
    public class ProductSpecialPropertyAddRequest
    {


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
