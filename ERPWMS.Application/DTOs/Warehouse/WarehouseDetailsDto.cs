using System;
using System.Collections.Generic;

namespace ERPWMS.Application.DTOs.Warehouse
{
    public class WarehouseDetailsDto
    {
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public string WarehouseCode { get; set; } = string.Empty;
        public int BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        // Audit Information
        public string CreatedBy { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy{ get; set; }
        public string UpdatedByName { get; set; } = string.Empty;
        public DateTime? UpdatedDate { get; set; }
        public string? DeletedBy { get; set; }
        public string DeletedByName { get; set; } = string.Empty;
        public DateTime? DeletedDate { get; set; }

        // Statistics
        public int ZoneCount { get; set; }
        public int RackCount { get; set; }
        public int BinCount { get; set; }
        public int OccupiedBinCount { get; set; }
        public int AvailableBinCount { get; set; }
        public int ActiveZoneCount { get; set; }
        public int ActiveRackCount { get; set; }
        public int ActiveBinCount { get; set; }

        // Navigation
        public List<ZoneSummaryDto> Zones { get; set; } = new();
    }
}

public class ZoneSummaryDto
    {
        public int ZoneId { get; set; }
        public string ZoneName { get; set; } = string.Empty;
        public string ZoneCode { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int RackCount { get; set; }
        public int BinCount { get; set; }
        public int OccupiedBinCount { get; set; }
    }

