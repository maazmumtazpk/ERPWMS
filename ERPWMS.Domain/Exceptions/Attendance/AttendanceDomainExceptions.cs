using System;
using ERPWMS.Database.Models;

namespace ERPWMS.Domain.Exceptions.Attendance
{
    public class AttendanceDomainException : Exception
    {
        public AttendanceDomainException(string message)
            : base(message)
        {
        }
    }

    public class AttendanceNotFoundException
        : AttendanceDomainException
    {
        public AttendanceNotFoundException()
            : base("Attendance record was not found.")
        {
        }
    }

    public class DuplicateAttendanceException
        : AttendanceDomainException
    {
        public DuplicateAttendanceException()
            : base("Attendance already exists for this employee and date.")
        {
        }
    }

    public class InvalidAttendanceTimeException
        : AttendanceDomainException
    {
        public InvalidAttendanceTimeException()
            : base("Check-out time cannot be earlier than check-in time.")
        {
        }
    }
}