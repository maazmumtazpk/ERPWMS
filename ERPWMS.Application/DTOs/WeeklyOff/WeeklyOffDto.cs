using System;

namespace ERPWMS.Application.DTOs.WeeklyOff
{
    public class WeeklyOffDto
    {
        public int WeeklyOffId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeCode { get; set; } = string.Empty;
        public string DayOfWeek { get; set; } = "Sunday";
        public DateOnly EffectiveFrom { get; set; }
        public DateOnly? EffectiveTo { get; set; }
        public string? Remarks { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
