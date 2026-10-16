using MediatR;
namespace ERPWMS.Application.DTOs.Warehouse
{
    public class GetWarehousesQuery : IRequest<PagedResult<WarehouseListDto>>
    {
        public string? SearchTerm { get; set; }
        public int? BranchId { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "WarehouseName";
        public string SortDirection { get; set; } = "ASC";
    }

    public class GetWarehouseDetailsQuery : IRequest<WarehouseDetailsDto>
    {
        public int WarehouseId { get; set; }
    }

    public class GetWarehouseDashboardQuery : IRequest<WarehouseDashboardDto>
    {
        public int? WarehouseId { get; set; }
    }

    public class GetWarehouseZonesQuery : IRequest<PagedResult<ZoneDto>>
    {
        public int WarehouseId { get; set; }
        public string? SearchTerm { get; set; }
        public bool? IsActive { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetWarehouseRacksQuery : IRequest<PagedResult<RackDto>>
    {
        public int WarehouseId { get; set; }
        public int? ZoneId { get; set; }
        public string? SearchTerm { get; set; }
        public bool? IsActive { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetWarehouseBinsQuery : IRequest<PagedResult<BinDto>>
    {
        public int WarehouseId { get; set; }
        public int? ZoneId { get; set; }
        public int? RackId { get; set; }
        public string? SearchTerm { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsOccupied { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetZonesQuery : IRequest<PagedResult<ZoneDto>>
    {
        public int? WarehouseId { get; set; }
        public string? SearchTerm { get; set; }
        public bool? IsActive { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetZoneDetailsQuery : IRequest<ZoneDto>
    {
        public int ZoneId { get; set; }
    }

    public class GetRacksQuery : IRequest<PagedResult<RackDto>>
    {
        public int? WarehouseId { get; set; }
        public int? ZoneId { get; set; }
        public string? SearchTerm { get; set; }
        public bool? IsActive { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetRackDetailsQuery : IRequest<RackDto>
    {
        public int RackId { get; set; }
    }

    public class GetBinsQuery : IRequest<PagedResult<BinDto>>
    {
        public int? WarehouseId { get; set; }
        public int? ZoneId { get; set; }
        public int? RackId { get; set; }
        public string? SearchTerm { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsOccupied { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetBinDetailsQuery : IRequest<BinDto>
    {
        public int BinId { get; set; }
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPrevious => PageNumber > 1;
        public bool HasNext => PageNumber < TotalPages;
    }
}

