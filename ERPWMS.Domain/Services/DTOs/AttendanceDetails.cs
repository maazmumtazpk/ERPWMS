namespace ERPWMS.Domain.Services.DTOs
{
    public class AttendanceDetails
    {
        public int AttendanceId { get; set; }
        public int EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmployeeCode { get; set; }
        public DateOnly AttendanceDate { get; set; }
        public TimeOnly? CheckInTime { get; set; }
        public TimeOnly? CheckOutTime { get; set; }
        public decimal? WorkingHours { get; set; }
        public decimal? OvertimeHours { get; set; }
        public string? Status { get; set; }
        public string? Remarks { get; set; }
        public int? ShiftId { get; set; }
        public string? ShiftName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}