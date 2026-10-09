using System;

namespace ERPWMS.Application.DTOs.Inventory
{
    public class CycleCountDetailDto
    {
        public int CountDetailId { get; set; }
        public int CountId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public int BinId { get; set; }
        public string BinName { get; set; } = string.Empty;
        public int SystemQty { get; set; }
        public int ActualQty { get; set; }
        public int Variance { get; set; }
        public string? Remarks { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}