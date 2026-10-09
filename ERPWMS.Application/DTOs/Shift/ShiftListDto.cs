using System;

namespace ERPWMS.Application.DTOs.Shift
{
    public class ShiftListDto
    {
        public int ShiftId { get; set; }
        public string ShiftName { get; set; } = null!;
        public string StartTime { get; set; } = null!;
        public string EndTime { get; set; } = null!;
        public int? GracePeriodMinutes { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public int EmployeeCount { get; set; }
    }
}
