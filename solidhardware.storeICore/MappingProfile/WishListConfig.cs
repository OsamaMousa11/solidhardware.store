using AutoMapper;
using solidhardware.storeCore.Domain.Entites;
using solidhardware.storeCore.DTO.WishListDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solidhardware.storeCore.MappingProfile
{
    public class WishListConfig: Profile
    {
        public WishListConfig()

        {
   
            CreateMap<WishListAddRequest, Wishlist>()
                .ForMember(dest => dest.WishlistItems, opt => opt.MapFrom(src => src.WishlistItems));

            CreateMap<WishlistItemAddRequest, WishlistItem>()
                .ForMember(dest => dest.WishlistId, opt => opt.Ignore());


            CreateMap<Wishlist, WishListResponse>()
                .ForMember(dest => dest.WishlistItems, opt => opt.MapFrom(src => src.WishlistItems));

            CreateMap<WishlistItem, WishlistItemResponse>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(src => src.Product.Price))
                .ForMember(dest => dest.ProductImageUrl, opt => opt.MapFrom(src => src.Product.ImageUrl));

  
            CreateMap<WishlistUpdateRequest, Wishlist>()
                
                .ForMember(dest => dest.WishlistItems, opt => opt.Ignore());

            CreateMap<WishlistItemUpdateRequest, WishlistItem>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId));










        }
    }
}
