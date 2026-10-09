using System;
using System.Threading.Tasks;
using ERPWMS.Database.Models;

namespace ERPWMS.Domain.Interfaces
{
    public interface IAttendanceProcessor
    {
        Task<AttendanceResult> ProcessAsync(AttendanceInput input);
    }

    public class AttendanceInput
    {
        public int EmployeeId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly? CheckIn { get; set; }
        public TimeOnly? CheckOut { get; set; }
        public int? ShiftId { get; set; }
        public int CurrentUserId { get; set; }
    }

    public class AttendanceResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int LateMinutes { get; set; }
        public int EarlyMinutes { get; set; }
        public decimal OTHours { get; set; }
        public int WorkingMinutes { get; set; }
        public bool IsHalfDay { get; set; }
        public bool IsHolidayWork { get; set; }
        public bool IsWeeklyOffWork { get; set; }
    }
}
