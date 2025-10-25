
using AutoMapper;
using solidhardware.storeCore.Domain.Entites;
using solidhardware.storeCore.DTO.CategotyDTO;

namespace solidhardware.storeCore.MappingProfile
{
    public class CategoryConfig:Profile
    {
        public CategoryConfig()
        {
            CreateMap<CategoryAddRequest,Category>().ReverseMap();
            CreateMap<CategoryUpdateRequest, Category>().ReverseMap();
            CreateMap<Category,CategoryResponse>().ReverseMap();


        }
    }
}
