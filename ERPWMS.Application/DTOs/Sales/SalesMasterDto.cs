using System;
using System.Collections.Generic;

namespace ERPWMS.Application.DTOs.Sales
{
    public class SalesMasterDto
    {
        public int SalesId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public string SalesOrderNo { get; set; } = string.Empty;
        public DateTime? OrderDate { get; set; }
        public DateOnly? DeliveryDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal? Discount { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal NetAmount { get; set; }
        public string? Status { get; set; }
        public string? ApprovalStatus { get; set; }
        public int? WorkflowInstanceId { get; set; }
        public int? CurrentStageId { get; set; }
        public string? CurrentStageName { get; set; }
        public string CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public String?UpdatedBy{ get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public List<SalesDetailDto>? SalesDetails { get; set; }
    }
}