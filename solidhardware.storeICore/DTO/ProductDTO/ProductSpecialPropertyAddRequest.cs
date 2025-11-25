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

      

    }
}
