using AutoMapper;
using solidhardware.storeCore.Domain.Entites;
using solidhardware.storeCore.DTO.CartDTO;

namespace solidhardware.storeCore.MappingProfile
{
    public class CartConfig : Profile
    {
        public CartConfig()
        {
            // Cartitem -> CartItemResponse
            CreateMap<Cartitem, CartItemResponse>()
                .ForMember(dest => dest.ProductName,
                    opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ImageUrl,
                    opt => opt.MapFrom(src => src.Product.ImageUrl))
                .ForMember(dest => dest.UnitPrice,
                    opt => opt.MapFrom(src => src.Product.Price))
                .ForMember(dest => dest.Total,
                    opt => opt.MapFrom(src => src.Quantity * src.Product.Price));

          
            CreateMap<Cart, CartResponse>()
                .ForMember(dest => dest.TotalItems,
                    opt => opt.MapFrom(src =>
                        src.CartItems != null ? src.CartItems.Sum(ci => ci.Quantity) : 0))
                .ForMember(dest => dest.TotalPrice,
                    opt => opt.MapFrom(src =>
                        src.CartItems != null
                            ? src.CartItems.Sum(ci => ci.Quantity * ci.Product.Price)
                            : 0))
                .ForMember(dest => dest.Items,
                    opt => opt.MapFrom(src => src.CartItems));
        }
    }
}
