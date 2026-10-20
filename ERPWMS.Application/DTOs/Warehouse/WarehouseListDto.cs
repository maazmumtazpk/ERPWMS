using System;

namespace ERPWMS.Application.DTOs.Warehouse
{
    public class WarehouseListDto
    {
        // =========================================================
        // WAREHOUSE
        // =========================================================

        public int WarehouseId { get; set; }

        public string WarehouseName { get; set; } = string.Empty;

        public string WarehouseCode { get; set; } = string.Empty;

        // =========================================================
        // BRANCH
        // =========================================================

        public int BranchId { get; set; }

        public string BranchName { get; set; } = string.Empty;

        // =========================================================
        // CONTACT / ADDRESS
        // =========================================================

        public string Address { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        // =========================================================
        // STATUS
        // =========================================================

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        // =========================================================
        // LOCATION COUNTS
        // =========================================================

        public int ZoneCount { get; set; }

        public int RackCount { get; set; }

        public int BinCount { get; set; }

        // =========================================================
        // AUDIT
        // =========================================================

        public DateTime? CreatedDate { get; set; }

        public string CreatedBy { get; set; }


        public DateTime? UpdatedDate { get; set; }
    }
}
