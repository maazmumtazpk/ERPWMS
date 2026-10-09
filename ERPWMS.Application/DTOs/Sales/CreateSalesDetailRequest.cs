
using System;

namespace ERPWMS.Application.DTOs.Sales
{
    public class CreateSalesDetailRequest
    {
        public int DetailId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal? DiscountPercent { get; set; }

        public string? BatchNo { get; set; }
    }
}

