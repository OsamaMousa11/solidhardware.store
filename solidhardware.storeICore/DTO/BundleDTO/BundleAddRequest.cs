using solidhardware.storeCore.Domain.Entites;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace solidhardware.storeCore.DTO.BundleDTO
{
    public class BundleAddRequest
    {
        [Required(ErrorMessage = "Bundle name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Bundle name must be between 3 and 100 characters")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description can't exceed 500 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Image URL is required")]
        [Url(ErrorMessage = "Invalid image URL format")]
        public string ImageUrl { get; set; }

        [Required(ErrorMessage = "At least one bundle item is required")]
        [MinLength(1, ErrorMessage = "Bundle must contain at least one product item")]
        public ICollection<BundleItemAddRequest> BundleItems { get; set; } 
    }
}
