using System;
using System.Collections.Generic;

namespace ERPWMS.Application.DTOs.Inventory
{
    public class CycleCountDto
    {
        public int CycleCountId { get; set; }
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public int BinId { get; set; }
        public string BinName { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public int SystemQty { get; set; }
        public int ActualQty { get; set; }
        public int Variance { get; set; }
        public int CountedBy { get; set; }
        public string? CountedByName { get; set; }
        public DateTime? CountedDate { get; set; }
        public int? VerifiedBy { get; set; }
        public string? VerifiedByName { get; set; }
        public DateTime? VerifiedDate { get; set; }
        public string? Status { get; set; }
        public string? Remarks { get; set; }
        public string CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public List<CycleCountDetailDto>? CycleCountDetails { get; set; }
    }
}