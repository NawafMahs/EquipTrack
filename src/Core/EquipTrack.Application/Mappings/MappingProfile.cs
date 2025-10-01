using AutoMapper;
using EquipTrack.Application.Assets.Commands;
using EquipTrack.Application.DTOs;
using EquipTrack.Domain.Entities;
using AssetEntity = EquipTrack.Domain.Entities.Asset;

namespace EquipTrack.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User mappings
        CreateMap<User, UserQuery>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName));

        // Asset Query mappings
        CreateMap<AssetEntity, AssetQuery>()
            .ForMember(dest => dest.IsUnderWarranty, opt => opt.MapFrom(src => src.IsUnderWarranty()))
            .ForMember(dest => dest.IsOperational, opt => opt.MapFrom(src => src.IsOperational()))
            .ForMember(dest => dest.ActiveWorkOrdersCount, opt => opt.MapFrom(src => 0)) // Will be calculated separately
            .ForMember(dest => dest.TotalWorkOrdersCount, opt => opt.MapFrom(src => 0)); // Will be calculated separately

        // Asset Command mappings
        CreateMap<CreateAssetCommand, AssetEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.WorkOrders, opt => opt.Ignore());

        CreateMap<UpdateAssetCommand, AssetEntity>()
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.WorkOrders, opt => opt.Ignore());

        // SparePart mappings
        CreateMap<SparePart, SparePartQuery>()
            .ForMember(dest => dest.IsLowStock, opt => opt.MapFrom(src => src.IsLowStock));

        // PreventiveMaintenance mappings
        CreateMap<PreventiveMaintenance, PreventiveMaintenanceQuery>()
            .ForMember(dest => dest.IsOverdue, opt => opt.MapFrom(src => src.IsOverdue))
            .ForMember(dest => dest.IsDueSoon, opt => opt.MapFrom(src => src.IsDueSoon));
    }
}