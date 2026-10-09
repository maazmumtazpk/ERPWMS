using System;

namespace ERPWMS.Application.DTOs.Leave
{
    public class LeaveDetails
    {
        public int LeaveId { get; set; }

        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; } = string.Empty;

        public string LeaveType { get; set; } = string.Empty;

        public DateOnly FromDate { get; set; }

        public DateOnly ToDate { get; set; }

        public int TotalDays { get; set; }

        public string Reason { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public int? ApprovedBy { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public string? RejectionReason { get; set; }

        public int? WorkflowInstanceId { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }

    public class LeaveBalanceDetails
    {
        public int EmployeeId { get; set; }

        public string LeaveType { get; set; } = string.Empty;

        public int Year { get; set; }

        public int AllowedDays { get; set; }

        public int UsedDays { get; set; }

        public int PendingDays { get; set; }

        public int RemainingDays { get; set; }
    }
}
