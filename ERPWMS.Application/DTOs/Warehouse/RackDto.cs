using System;

namespace ERPWMS.Application.DTOs.Warehouse
{
    public class RackDto
    {
        public int RackId { get; set; }

        // ✅ Added this
        public int WarehouseId { get; set; }

        public int ZoneId { get; set; }
        public string ZoneName { get; set; } = string.Empty;
        public string WarehouseName { get; set; } = string.Empty;
        public string RackName { get; set; } = string.Empty;
        public string RackCode { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        // Statistics
        public int BinCount { get; set; }
        public int OccupiedBinCount { get; set; }
        public int AvailableBinCount { get; set; }

        // Audit
        public string CreatedBy { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }
        public String?UpdatedBy{ get; set; }
        public string UpdatedByName { get; set; } = string.Empty;
        public DateTime? UpdatedDate { get; set; }
    }
}