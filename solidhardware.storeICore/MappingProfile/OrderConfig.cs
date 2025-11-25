using AutoMapper;
using solidhardware.storeCore.Domain.Entites;
using solidhardware.storeCore.DTO.OrderDTO;

public class OrderConfig : Profile
{
    public OrderConfig()
    {
        // -----------------------------
        // CREATE (Add Request → Entity)
        // -----------------------------
        CreateMap<OrderAddRequest, Order>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.TotalAmount, opt => opt.Ignore());

        CreateMap<OrderItemAddRequest, OrderItem>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UnitPrice, opt => opt.Ignore());
         

        // -----------------------------
        // RESPONSE (Entity → Response)
        // -----------------------------
        CreateMap<Order, OrderResponse>()
           
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderItems));

        CreateMap<OrderItem, OrderItemResponse>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Product.ImageUrl))
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Quantity * src.UnitPrice));
    }
}
