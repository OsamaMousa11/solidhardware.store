using AutoMapper;
using solidhardware.storeCore.Domain.Entites;
using solidhardware.storeCore.DTO.ProductDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solidhardware.storeCore.MappingProfile
{
    public class ProductConfig:Profile
    {
        public ProductConfig()
        {

            CreateMap<ProductAddRequest, Product>().ForMember(dest => dest.BundleItems, opt => opt.Ignore())
            .ForMember(dest => dest.ProductSpecialProperty, opt => opt.MapFrom(src => src.SpecialProperties));

            CreateMap<Product, ProductResponse>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.SpecialProperties, opt => opt.MapFrom(src => src.ProductSpecialProperty));

            CreateMap<ProductSpecialPropertyAddRequest, ProductSpecialProperty>()
             .ForMember(dest => dest.ProductId, opt => opt.Ignore()) 
             .ForMember(dest => dest.Product, opt => opt.Ignore());

            CreateMap<ProductSpecialProperty, ProductSpecialPropertyResponse>();

            CreateMap<ProductUpdateRequest, Product>()
         .ForMember(dest => dest.BundleItems, opt => opt.Ignore())
         .ForMember(dest => dest.ProductSpecialProperty, opt => opt.MapFrom(src => src.SpecialProperties));

            CreateMap<ProductSpecialPropertyUpdateRequest, ProductSpecialProperty>()
                .ForMember(dest => dest.ProductId, opt => opt.Ignore())
                .ForMember(dest => dest.Product, opt => opt.Ignore());






        }
    }
}
