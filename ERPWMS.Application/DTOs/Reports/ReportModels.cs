using System;
using System.Collections.Generic;

namespace ERPWMS.Application.DTOs.Reports
{
    public class ReportParameter
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? WarehouseId { get; set; }
        public int? ProductId { get; set; }
        public int? EmployeeId { get; set; }
        public int? CustomerId { get; set; }
        public int? SupplierId { get; set; }
        public string? TransactionType { get; set; }
        public string? Status { get; set; }
        public string? SearchTerm { get; set; }
    }

    public class ReportResult
    {
        public string ReportCode { get; set; } = "";
        public string ReportName { get; set; } = "";
        public List<string> Columns { get; set; } = new();
        public List<Dictionary<string, object>> Rows { get; set; } = new();
        public int TotalRows { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.Now;
    }

    public class ReportDefinitionDto
    {
        public string ReportCode { get; set; } = "";
        public string ReportName { get; set; } = "";
        public string ReportCategory { get; set; } = "";
        public string Description { get; set; } = "";
    }
}
