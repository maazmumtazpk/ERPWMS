using System;

namespace ERPWMS.Application.DTOs.Sales
{
    public class PickingDetailDto
    {
        public int PickingDetailId { get; set; }
        public int PickingId { get; set; }
        public int SalesDetailId { get; set; }
        public int ProductId { get; set; }

        public string? ProductName { get; set; }
        public string? ProductCode { get; set; }

        public int? BinId { get; set; }              // ✅ int? — FIX
        public string? BinName { get; set; }

        public int PickedQty { get; set; }
        public string? Status { get; set; }
        public string? BatchNo { get; set; }
        public string? SerialNumbers { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}