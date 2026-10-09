using AutoMapper;
using ERPWMS.Application.DTOs.Warehouse;
using ERPWMS.Database.Models;
using System.Linq;

namespace ERPWMS.Application.Mappings
{
    public class WarehouseMappingProfile : Profile
    {
        public WarehouseMappingProfile()
        {
            // =========================================================
            // Warehouse List Mapping
            // =========================================================

            CreateMap<Warehouse, WarehouseListDto>()
                .ForMember(
                    dest => dest.BranchName,
                    opt => opt.MapFrom(src =>
                        src.Branch != null
                            ? src.Branch.BranchName
                            : null))
                .ForMember(
                    dest => dest.ZoneCount,
                    opt => opt.MapFrom(src =>
                        src.Zones.Count))
                .ForMember(
                    dest => dest.RackCount,
                    opt => opt.MapFrom(src =>
                        src.Zones
                            .SelectMany(z => z.Racks)
                            .Count()))
                .ForMember(
                    dest => dest.BinCount,
                    opt => opt.MapFrom(src =>
                        src.Zones
                            .SelectMany(z => z.Racks)
                            .SelectMany(r => r.Bins)
                            .Count()))
                .ForMember(
                    dest => dest.CreatedByName,
                    opt => opt.MapFrom(src =>
                        src.CreatedByNavigation != null
                            ? src.CreatedByNavigation.UserName
                            : null));

            // =========================================================
            // Warehouse Details Mapping
            // =========================================================

            CreateMap<Warehouse, WarehouseDetailsDto>()
                .ForMember(
                    dest => dest.BranchName,
                    opt => opt.MapFrom(src =>
                        src.Branch != null
                            ? src.Branch.BranchName
                            : null))
                .ForMember(
                    dest => dest.CreatedByName,
                    opt => opt.MapFrom(src =>
                        src.CreatedByNavigation != null
                            ? src.CreatedByNavigation.UserName
                            : null))
                .ForMember(
                    dest => dest.UpdatedByName,
                    opt => opt.MapFrom(src =>
                        src.UpdatedByNavigation != null
                            ? src.UpdatedByNavigation.UserName
                            : null))
                .ForMember(
                    dest => dest.DeletedByName,
                    opt => opt.MapFrom(src =>
                        src.DeletedByNavigation != null
                            ? src.DeletedByNavigation.UserName
                            : null))
                .ForMember(
                    dest => dest.ZoneCount,
                    opt => opt.MapFrom(src =>
                        src.Zones.Count))
                .ForMember(
                    dest => dest.RackCount,
                    opt => opt.MapFrom(src =>
                        src.Zones
                            .SelectMany(z => z.Racks)
                            .Count()))
                .ForMember(
                    dest => dest.BinCount,
                    opt => opt.MapFrom(src =>
                        src.Zones
                            .SelectMany(z => z.Racks)
                            .SelectMany(r => r.Bins)
                            .Count()))
                .ForMember(
                    dest => dest.OccupiedBinCount,
                    opt => opt.MapFrom(src =>
                        src.Zones
                            .SelectMany(z => z.Racks)
                            .SelectMany(r => r.Bins)
                            .Count(b => b.IsOccupied == true)))
                .ForMember(
                    dest => dest.AvailableBinCount,
                    opt => opt.MapFrom(src =>
                        src.Zones
                            .SelectMany(z => z.Racks)
                            .SelectMany(r => r.Bins)
                            .Count(b =>
                                b.IsOccupied == false ||
                                b.IsOccupied == null)))
                .ForMember(
                    dest => dest.ActiveZoneCount,
                    opt => opt.MapFrom(src =>
                        src.Zones.Count(z => z.IsActive == true)))
                .ForMember(
                    dest => dest.ActiveRackCount,
                    opt => opt.MapFrom(src =>
                        src.Zones
                            .SelectMany(z => z.Racks)
                            .Count(r => r.IsActive == true)))
                .ForMember(
                    dest => dest.ActiveBinCount,
                    opt => opt.MapFrom(src =>
                        src.Zones
                            .SelectMany(z => z.Racks)
                            .SelectMany(r => r.Bins)
                            .Count(b => b.IsActive == true)))
                .ForMember(
                    dest => dest.Zones,
                    opt => opt.MapFrom(src =>
                        src.Zones.Where(z => z.IsDeleted != true)));

            // =========================================================
            // Zone Mapping
            // =========================================================

            CreateMap<Zone, ZoneDto>()
                .ForMember(
                    dest => dest.WarehouseName,
                    opt => opt.MapFrom(src =>
                        src.Warehouse != null
                            ? src.Warehouse.WarehouseName
                            : null))
                .ForMember(
                    dest => dest.RackCount,
                    opt => opt.MapFrom(src =>
                        src.Racks.Count))
                .ForMember(
                    dest => dest.BinCount,
                    opt => opt.MapFrom(src =>
                        src.Racks
                            .SelectMany(r => r.Bins)
                            .Count()))
                .ForMember(
                    dest => dest.OccupiedBinCount,
                    opt => opt.MapFrom(src =>
                        src.Racks
                            .SelectMany(r => r.Bins)
                            .Count(b => b.IsOccupied == true)))
                .ForMember(
                    dest => dest.AvailableBinCount,
                    opt => opt.MapFrom(src =>
                        src.Racks
                            .SelectMany(r => r.Bins)
                            .Count(b =>
                                b.IsOccupied == false ||
                                b.IsOccupied == null)))
                .ForMember(
                    dest => dest.CreatedByName,
                    opt => opt.MapFrom(src =>
                        src.CreatedByNavigation != null
                            ? src.CreatedByNavigation.UserName
                            : null))
                .ForMember(
                    dest => dest.UpdatedByName,
                    opt => opt.MapFrom(src =>
                        src.UpdatedByNavigation != null
                            ? src.UpdatedByNavigation.UserName
                            : null));

            // =========================================================
            // Rack Mapping
            // =========================================================

            CreateMap<Rack, RackDto>()
                .ForMember(
                    dest => dest.ZoneName,
                    opt => opt.MapFrom(src =>
                        src.Zone != null
                            ? src.Zone.ZoneName
                            : null))
                .ForMember(
                    dest => dest.WarehouseName,
                    opt => opt.MapFrom(src =>
                        src.Zone != null &&
                        src.Zone.Warehouse != null
                            ? src.Zone.Warehouse.WarehouseName
                            : null))
                .ForMember(
                    dest => dest.BinCount,
                    opt => opt.MapFrom(src =>
                        src.Bins.Count))
                .ForMember(
                    dest => dest.OccupiedBinCount,
                    opt => opt.MapFrom(src =>
                        src.Bins.Count(b => b.IsOccupied == true)))
                .ForMember(
                    dest => dest.AvailableBinCount,
                    opt => opt.MapFrom(src =>
                        src.Bins.Count(b =>
                            b.IsOccupied == false ||
                            b.IsOccupied == null)))
                .ForMember(
                    dest => dest.CreatedByName,
                    opt => opt.MapFrom(src =>
                        src.CreatedByNavigation != null
                            ? src.CreatedByNavigation.UserName
                            : null))
                .ForMember(
                    dest => dest.UpdatedByName,
                    opt => opt.MapFrom(src =>
                        src.UpdatedByNavigation != null
                            ? src.UpdatedByNavigation.UserName
                            : null));

            // =========================================================
            // Bin Mapping
            // =========================================================

            CreateMap<Bin, BinDto>()
                .ForMember(
                    dest => dest.RackName,
                    opt => opt.MapFrom(src =>
                        src.Rack != null
                            ? src.Rack.RackName
                            : null))
                .ForMember(
                    dest => dest.ZoneName,
                    opt => opt.MapFrom(src =>
                        src.Rack != null &&
                        src.Rack.Zone != null
                            ? src.Rack.Zone.ZoneName
                            : null))
                .ForMember(
                    dest => dest.WarehouseName,
                    opt => opt.MapFrom(src =>
                        src.Rack != null &&
                        src.Rack.Zone != null &&
                        src.Rack.Zone.Warehouse != null
                            ? src.Rack.Zone.Warehouse.WarehouseName
                            : null))
                .ForMember(
                    dest => dest.CreatedByName,
                    opt => opt.MapFrom(src =>
                        src.CreatedByNavigation != null
                            ? src.CreatedByNavigation.UserName
                            : null))
                .ForMember(
                    dest => dest.UpdatedByName,
                    opt => opt.MapFrom(src =>
                        src.UpdatedByNavigation != null
                            ? src.UpdatedByNavigation.UserName
                            : null));

            // =========================================================
            // Zone Summary Mapping
            // =========================================================

            CreateMap<Zone, ZoneSummaryDto>()
                .ForMember(
                    dest => dest.RackCount,
                    opt => opt.MapFrom(src =>
                        src.Racks.Count))
                .ForMember(
                    dest => dest.BinCount,
                    opt => opt.MapFrom(src =>
                        src.Racks
                            .SelectMany(r => r.Bins)
                            .Count()))
                .ForMember(
                    dest => dest.OccupiedBinCount,
                    opt => opt.MapFrom(src =>
                        src.Racks
                            .SelectMany(r => r.Bins)
                            .Count(b => b.IsOccupied == true)));
        }
    }
}