using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solidhardware.storeCore.DTO.CategotyDTO
{
    public class CategoryAddRequest
    {
        [Required(ErrorMessage = "Category name can't be blank")]
        [MaxLength(20, ErrorMessage = "Category name can't exceed 20 characters")]
        [MinLength(3, ErrorMessage = "Category name must be at least 3 characters")]

        public string? Name { get; set; }


    }
}
