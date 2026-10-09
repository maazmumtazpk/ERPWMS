using System;

namespace ERPWMS.Application.DTOs.Procurement
{
    public class GrndetailDto
    {
        public int GrndetailId { get; set; }
        public int GrnId { get; set; }
        public int PurchaseDetailId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public int BinId { get; set; }
        public string BinName { get; set; } = string.Empty;
        public int ReceivedQty { get; set; }
        public int AcceptedQty { get; set; }
        public int? RejectedQty { get; set; }
        public string? RejectionReason { get; set; }
        public string? BatchNo { get; set; }
        public DateOnly? ExpiryDate { get; set; }
        public string? SerialNumbers { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public String?UpdatedBy{ get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}