using AutoMapper;
using solidhardware.storeCore.Domain.Entites;
using solidhardware.storeCore.DTO.BundleDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace solidhardware.storeCore.MappingProfile
{
    public class BundleConfig:Profile
    {
        public BundleConfig()
        {
            CreateMap<BundleAddRequest, Bundle>()
                .ForMember(dest => dest.BundleItems,opt=>opt.MapFrom(src=>src.BundleItems));

            CreateMap<BundleItemAddRequest, BundleItem>()
                .ForMember(dest => dest.Bundle, opt => opt.Ignore())
            .ForMember(dest => dest.BundleId, opt => opt.Ignore());


            CreateMap<Bundle, BundleResponse>()
                .ForMember(dest => dest.BundleItems, opt => opt.MapFrom(src => src.BundleItems));

            CreateMap<BundleItem, BundleItemResponse>().ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name)); 

            CreateMap<BundleUpdateRequest, Bundle>()
           .ForMember(dest => dest.BundleItems, opt => opt.MapFrom(src => src.BundleItems));

            CreateMap<BundleItemUpdateRequest, BundleItem>()
             
              .ForMember(dest => dest.Bundle, opt => opt.Ignore())
          .ForMember(dest => dest.BundleId, opt => opt.Ignore());



        }
    }
}
