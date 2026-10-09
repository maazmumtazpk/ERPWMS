using System;

namespace ERPWMS.Application.DTOs.Leave
{
    public class SubmitLeaveRequest
    {
        public int EmployeeId { get; set; }

        public string LeaveType { get; set; } = string.Empty;

        public DateOnly FromDate { get; set; }

        public DateOnly ToDate { get; set; }

        public string Reason { get; set; } = string.Empty;
    }

    public class ApproveLeaveRequest
    {
        public int LeaveId { get; set; }
    }

    public class RejectLeaveRequest
    {
        public int LeaveId { get; set; }

        public string RejectionReason { get; set; } = string.Empty;
    }

    public class CancelLeaveRequest
    {
        public int LeaveId { get; set; }
    }
}