using System;

namespace ERPWMS.Application.DTOs.Shift
{
    public class CreateShiftRequest
    {
        public string ShiftName { get; set; } = null!;

        public TimeOnly? StartTime { get; set; }

        public TimeOnly? EndTime { get; set; }

        public int? GracePeriodMinutes { get; set; }
    }
}