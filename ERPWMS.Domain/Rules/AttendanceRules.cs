using System;
using ERPWMS.Database.Models;

namespace ERPWMS.Domain.Rules
{
    public static class AttendanceRules
    {
        public static void ValidateCheckIn(
            int employeeId,
            DateOnly attendanceDate,
            TimeOnly checkInTime)
        {
            if (employeeId <= 0)
                throw new ArgumentException("Employee is required.");

            if (checkInTime == default)
                throw new ArgumentException("Check-in time is required.");
        }

        public static void ValidateCheckOut(
            int employeeId,
            TimeOnly checkInTime,
            TimeOnly checkOutTime)
        {
            if (employeeId <= 0)
                throw new ArgumentException("Employee is required.");

            if (checkOutTime < checkInTime)
                throw new ArgumentException(
                    "Check-out time cannot be earlier than check-in time.");
        }

        public static decimal CalculateWorkingHours(
            TimeOnly checkInTime,
            TimeOnly checkOutTime)
        {
            if (checkOutTime < checkInTime)
                throw new ArgumentException(
                    "Invalid attendance time range.");

            return (decimal)(checkOutTime - checkInTime).TotalHours;
        }

        public static decimal CalculateOvertimeHours(
            decimal workingHours,
            decimal standardWorkingHours = 8)
        {
            if (workingHours <= standardWorkingHours)
                return 0;

            return workingHours - standardWorkingHours;
        }
    }
}