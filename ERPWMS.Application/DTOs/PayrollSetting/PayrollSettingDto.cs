using System;
using System.ComponentModel.DataAnnotations;

namespace ERPWMS.Application.DTOs.PayrollSetting
{
    public class PayrollSettingDto
    {
        public int SettingId { get; set; }
        public string Frequency { get; set; } = "Monthly";
        [Range(1, 31)] public int SalaryDivisor { get; set; } = 30;
        [Range(1, 31)] public int WorkingDays { get; set; } = 26;
        public int GracePeriodMinutes { get; set; } = 15;
        public int LateThresholdMinutes { get; set; } = 30;
        public int EarlyThresholdMinutes { get; set; } = 30;
        [Range(1.0, 5.0)] public decimal OTMultiplier { get; set; } = 1.5m;
        public string RoundingRule { get; set; } = "Nearest";
        public string Currency { get; set; } = "PKR";
        public bool ApprovalRequired { get; set; } = true;
        public bool IsActive { get; set; } = true;
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}

