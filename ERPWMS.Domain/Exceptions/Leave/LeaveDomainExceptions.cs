using System;
using ERPWMS.Database.Models;

namespace ERPWMS.Domain.Exceptions.Leave
{
    public class LeaveDomainException
        : Exception
    {
        public LeaveDomainException(
            string message)
            : base(message)
        {
        }
    }

    public class LeaveNotFoundException
        : LeaveDomainException
    {
        public LeaveNotFoundException()
            : base("Leave request was not found.")
        {
        }
    }

    public class InvalidLeaveStatusException
        : LeaveDomainException
    {
        public InvalidLeaveStatusException(
            string message)
            : base(message)
        {
        }
    }

    public class LeaveOverlapException
        : LeaveDomainException
    {
        public LeaveOverlapException()
            : base(
                "Employee already has a leave request for the selected dates.")
        {
        }
    }

    public class InvalidLeaveDateException
        : LeaveDomainException
    {
        public InvalidLeaveDateException()
            : base(
                "Leave From Date cannot be greater than To Date.")
        {
        }
    }
}