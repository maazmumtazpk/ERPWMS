using System;

namespace ERPWMS.Domain.Services.DTOs
{
    public class GetLeaveHistoryRequest
    {
        public int EmployeeId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Status { get; set; }
    }
}