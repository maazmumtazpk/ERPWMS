using System;

namespace ERPWMS.Domain.Exceptions.HRMaster
{
    // ============================================================
    // BASE SHIFT DOMAIN EXCEPTION
    // ============================================================

    public class ShiftDomainException : HRMasterDomainException
    {
        public ShiftDomainException(
            string message,
            string errorCode)
            : base(message, errorCode)
        {
        }

        public ShiftDomainException(
            string message,
            string errorCode,
            Exception innerException)
            : base(message, errorCode, innerException)
        {
        }
    }


    // ============================================================
    // SHIFT NOT FOUND
    // ============================================================

    public class ShiftNotFoundException : ShiftDomainException
    {
        public int? ShiftId { get; }
        public string? ShiftName { get; }

        public ShiftNotFoundException(int shiftId)
            : base(
                $"Shift with ID {shiftId} was not found",
                "SFT-001")
        {
            ShiftId = shiftId;
        }

        public ShiftNotFoundException(string shiftName)
            : base(
                $"Shift with name '{shiftName}' was not found",
                "SFT-001")
        {
            ShiftName = shiftName;
        }
    }


    // ============================================================
    // SHIFT ALREADY EXISTS
    // ============================================================

    public class ShiftAlreadyExistsException : ShiftDomainException
    {
        public string ShiftName { get; }

        public ShiftAlreadyExistsException(string shiftName)
            : base(
                $"Shift with name '{shiftName}' already exists",
                "SFT-002")
        {
            ShiftName = shiftName;
        }
    }


    // ============================================================
    // SHIFT INACTIVE
    // ============================================================

    public class ShiftInactiveException : ShiftDomainException
    {
        public int ShiftId { get; }

        public ShiftInactiveException(int shiftId)
            : base(
                $"Shift with ID {shiftId} is inactive",
                "SFT-003")
        {
            ShiftId = shiftId;
        }
    }


    // ============================================================
    // SHIFT DELETED
    // ============================================================

    public class ShiftDeletedException : ShiftDomainException
    {
        public int ShiftId { get; }

        public ShiftDeletedException(int shiftId)
            : base(
                $"Shift with ID {shiftId} has been deleted and cannot be modified",
                "SFT-004")
        {
            ShiftId = shiftId;
        }
    }


    // ============================================================
    // SHIFT TIME CONFLICT
    // ============================================================

    public class ShiftTimeConflictException : ShiftDomainException
    {
        public TimeOnly StartTime { get; }
        public TimeOnly EndTime { get; }

        public ShiftTimeConflictException(
            TimeOnly startTime,
            TimeOnly endTime)
            : base(
                $"Start time {startTime} must be before end time {endTime}",
                "SFT-005")
        {
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}