using solidhardware.storeCore.Domain.Entites;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solidhardware.storeCore.DTO.WishListDTO
{
    public  class WishlistItemAddRequest
    {

        [Required(ErrorMessage = "ProductId is required")]
        public Guid ProductId { get; set; }
    }
    }

