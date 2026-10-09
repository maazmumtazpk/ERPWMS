using System;
using System.Collections.Generic;

namespace ERPWMS.Application.DTOs.Payroll
{
    public class PayrollDetailDto
    {
        public int PayrollId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;

        public DateOnly MonthYear { get; set; }

        public decimal GrossPay { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal NetPay { get; set; }

        public string Status { get; set; } = string.Empty;

        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? PaymentDate { get; set; }

        public List<PayrollComponentDto> Components { get; set; }
            = new();
    }

    public class PayrollComponentDto
    {
        public int PayrollDetailId { get; set; }
        public int ComponentId { get; set; }
        public string ComponentName { get; set; } = string.Empty;
        public string ComponentType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }

    public class PayrollHistoryDto
    {
        public int PayrollId { get; set; }
        public DateOnly MonthYear { get; set; }
        public decimal GrossPay { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal NetPay { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class PayrollReportDto
    {
        public int PayrollId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public DateOnly MonthYear { get; set; }
        public decimal GrossPay { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal NetPay { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
