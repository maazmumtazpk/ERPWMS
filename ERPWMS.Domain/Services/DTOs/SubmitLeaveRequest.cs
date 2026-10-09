using System;
using System.Collections.Generic;

namespace ERPWMS.Domain.Services.DTOs
{
    public class SubmitLeaveRequest
    {
        public int EmployeeId { get; set; }
        public int LeaveTypeId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string? Reason { get; set; }
        public string? RequestedBy { get; set; }
    }
}