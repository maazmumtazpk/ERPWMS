using System;

namespace ERPWMS.Domain.Rules
{
    public static class LeaveRules
    {
        public static readonly string[] AllowedLeaveTypes =
        {
            "Annual",
            "Sick",
            "Casual",
            "Maternity",
            "Paternity",
            "Unpaid"
        };

        public static bool IsValidLeaveType(
            string? leaveType)
        {
            if (string.IsNullOrWhiteSpace(leaveType))
                return false;

            return Array.Exists(
                AllowedLeaveTypes,
                x => x.Equals(
                    leaveType,
                    StringComparison.OrdinalIgnoreCase));
        }

        public static bool IsValidDateRange(
            DateOnly fromDate,
            DateOnly toDate)
        {
            return fromDate <= toDate;
        }

        public static int CalculateTotalDays(
            DateOnly fromDate,
            DateOnly toDate)
        {
            if (!IsValidDateRange(fromDate, toDate))
                return 0;

            return toDate.DayNumber -
                   fromDate.DayNumber + 1;
        }

        public static int GetDefaultAllowedDays(
            string leaveType)
        {
            return leaveType.ToLowerInvariant() switch
            {
                "annual" => 14,
                "sick" => 10,
                "casual" => 10,
                "maternity" => 90,
                "paternity" => 7,
                "unpaid" => 365,
                _ => 0
            };
        }
    }
}
