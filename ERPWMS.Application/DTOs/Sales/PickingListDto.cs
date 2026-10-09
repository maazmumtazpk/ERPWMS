using System;
using System.Collections.Generic;

namespace ERPWMS.Application.DTOs.Sales
{
    public class PickingListDto
    {
        public int PickingId { get; set; }
        public int SalesId { get; set; }
        public string? SalesOrderNo { get; set; }

        public int WarehouseId { get; set; }
        public string? WarehouseName { get; set; }

        public string? PickingNo { get; set; }
        public DateTime? PickingDate { get; set; }
        public string? Status { get; set; }

        public int? AssignedTo { get; set; }
        public string? AssignedToName { get; set; }

        public string? PickedBy { get; set; }          // ✅ string
        public string? PickedByName { get; set; }

        public string? VerifiedBy { get; set; }        // ✅ string
        public string? VerifiedByName { get; set; }

        public string? CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime? CreatedDate { get; set; }

        public bool IsDeleted { get; set; }

        public List<PickingDetailDto> PickingDetails { get; set; } = new List<PickingDetailDto>();
    }

   
}