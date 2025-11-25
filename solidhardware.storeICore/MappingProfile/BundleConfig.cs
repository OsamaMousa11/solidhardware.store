using AutoMapper;
using solidhardware.storeCore.Domain.Entites;
using solidhardware.storeCore.DTO.BundleDTO;

public class BundleConfig : Profile
{
    public BundleConfig()
    {
        // Bundle Mappings
        CreateMap<BundleAddRequest, Bundle>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) 
            .ForMember(dest => dest.BundleItems, opt => opt.MapFrom(src => src.BundleItems));

        CreateMap<Bundle, BundleResponse>()
            .ForMember(dest => dest.BundleItems, opt => opt.MapFrom(src => src.BundleItems));

        CreateMap<BundleUpdateRequest, Bundle>()
            .ForMember(dest => dest.BundleItems, opt => opt.MapFrom(src => src.BundleItems));

   
        CreateMap<BundleItemAddRequest, BundleItem>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) 
            .ForMember(dest => dest.Bundle, opt => opt.Ignore())
            .ForMember(dest => dest.BundleId, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore());

        CreateMap<BundleItem, BundleItemResponse>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));

        CreateMap<BundleItemUpdateRequest, BundleItem>()
            .ForMember(dest => dest.Bundle, opt => opt.Ignore())
            .ForMember(dest => dest.BundleId, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore());
    }
}