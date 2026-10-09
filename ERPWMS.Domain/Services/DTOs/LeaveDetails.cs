using System;

namespace ERPWMS.Domain.Services.DTOs
{
    public class LeaveDetails
    {
        public int LeaveId { get; set; }
        public int EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public int LeaveTypeId { get; set; }
        public string? LeaveTypeName { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int TotalDays { get; set; }
        public string? Reason { get; set; }
        public string? Status { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}