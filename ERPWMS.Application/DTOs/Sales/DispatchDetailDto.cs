
using System;

namespace ERPWMS.Application.DTOs.Sales
{
    public class DispatchDetailDto
    {
        public int DispatchDetailId { get; set; }

        public int DispatchId { get; set; }

        public int SalesDetailId { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string ProductCode { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public string? BatchNo { get; set; }

        public string? SerialNumbers { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }
    }
}


