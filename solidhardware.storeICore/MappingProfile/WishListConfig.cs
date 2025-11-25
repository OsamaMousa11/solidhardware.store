using AutoMapper;
using solidhardware.storeCore.Domain.Entites;
using solidhardware.storeCore.DTO.WishListDTO;

namespace solidhardware.storeCore.MappingProfile
{
    public class WishListConfig : Profile
    {
        public WishListConfig()
        {
            // ADD REQUEST -> ENTITY
            CreateMap<WishListAddRequest, Wishlist>()
                .ForMember(dest => dest.WishlistItems, opt => opt.Ignore());

         

            // ENTITY -> RESPONSE
            CreateMap<Wishlist, WishListResponse>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.WishlistItems));

            CreateMap<WishlistItem, WishListItemResponse>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Product.ImageUrl))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Product.Price));
        }
    }
}
