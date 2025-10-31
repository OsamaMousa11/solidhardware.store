using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solidhardware.storeCore.DTO.ProductDTO
{
    public class ProductUpdateRequest
    {
        [Required(ErrorMessage = "Id is required")]
        public Guid Id { get; set; }  

        [Required(ErrorMessage = "Product name can't be blank")]
        [MaxLength(200, ErrorMessage = "Product name can't exceed 200 characters")]
        [MinLength(3, ErrorMessage = "Product name must be at least 3 characters")]
        public string? Name { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "Brand can't be blank")]
        public string Brand { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Stock quantity is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative")]
        public int Stock_quantity { get; set; }

        [Url(ErrorMessage = "Invalid image URL format")]
        public string? ImageUrl { get; set; }

        [Required(ErrorMessage = "Category ID is required")]
        public Guid CategoryId { get; set; }

        public ICollection<ProductSpecialPropertyUpdateRequest>? SpecialProperties { get; set; }
    }
}
