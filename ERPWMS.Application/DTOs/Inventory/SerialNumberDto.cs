using System;

namespace ERPWMS.Application.DTOs.Inventory
{
    public class SerialNumberDto
    {
        public int SerialId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public int GrndetailId { get; set; }
        public string SerialNo { get; set; } = string.Empty;
        public string? Status { get; set; }
        public int? SalesDetailId { get; set; }
        public int? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public DateTime? OutDate { get; set; }
        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }
        public string StatusDisplay => Status switch
        {
            "IN" => "✅ Available",
            "Reserved" => "🔒 Reserved",
            "Issued" => "📤 Issued",
            "Sold" => "💰 Sold",
            "Returned" => "↩️ Returned",
            _ => "❓ Unknown"
        };
    }
}
