using System;

namespace ERPWMS.Application.DTOs.Shift
{
    public class ShiftDto
    {
        public int ShiftId { get; set; }
        public string ShiftName { get; set; } = null!;
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int? GracePeriodMinutes { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public string? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy{ get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int EmployeeCount { get; set; }
    }
}

