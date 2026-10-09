using System.Collections.Generic;

namespace ERPWMS.Application.DTOs.Warehouse
{
    public class WarehouseDashboardDto
    {
        // Summary Statistics
        public int TotalWarehouses { get; set; }
        public int ActiveWarehouses { get; set; }
        public int InactiveWarehouses { get; set; }
        public int TotalZones { get; set; }
        public int ActiveZones { get; set; }
        public int TotalRacks { get; set; }
        public int ActiveRacks { get; set; }
        public int TotalBins { get; set; }
        public int ActiveBins { get; set; }
        public int OccupiedBins { get; set; }
        public int AvailableBins { get; set; }
        public decimal OccupancyRate { get; set; }

        // Distribution Data
        public List<WarehouseDistributionDto> WarehouseDistribution { get; set; } = new();
        public List<RecentActivityDto> RecentActivities { get; set; } = new();
    }

    public class WarehouseDistributionDto
    {
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public string WarehouseCode { get; set; } = string.Empty;
        public int ZoneCount { get; set; }
        public int RackCount { get; set; }
        public int BinCount { get; set; }
        public int OccupiedBins { get; set; }
        public bool IsActive { get; set; }
    }

    public class RecentActivityDto
    {
        public string ActivityType { get; set; } = string.Empty;
        public string EntityName { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime ActivityDate { get; set; }
    }
}
