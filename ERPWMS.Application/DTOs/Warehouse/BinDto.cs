using System;

namespace ERPWMS.Application.DTOs.Warehouse
{
    public class BinDto
    {
        public int BinId { get; set; }

        // ✅ Added these two properties
        public int WarehouseId { get; set; }
        public int ZoneId { get; set; }

        public int RackId { get; set; }
        public string RackName { get; set; } = string.Empty;
        public string ZoneName { get; set; } = string.Empty;
        public string WarehouseName { get; set; } = string.Empty;
        public string BinName { get; set; } = string.Empty;
        public string BinCode { get; set; } = string.Empty;
        public bool? IsOccupied { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        // Audit
        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy{ get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}

