using System;

namespace ERPWMS.Domain.Exceptions.HRMaster
{
    public abstract class HRMasterDomainException : Exception
    {
        public string ErrorCode { get; }

        protected HRMasterDomainException(
            string message,
            string errorCode)
            : base(message)
        {
            ErrorCode = errorCode;
        }

        protected HRMasterDomainException(
            string message,
            string errorCode,
            Exception innerException)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
        }
    }

    // ============================================================
    // HR MASTER VALIDATION EXCEPTION
    // ============================================================

    public class HRMasterValidationException : HRMasterDomainException
    {
        public HRMasterValidationException(
            string message,
            string errorCode = "HR-099")
            : base(message, errorCode)
        {
        }

        public HRMasterValidationException(
            string message,
            string errorCode,
            Exception innerException)
            : base(message, errorCode, innerException)
        {
        }
    }

    // ============================================================
    // UNAUTHORIZED HR MASTER ACCESS
    // ============================================================

    public class UnauthorizedHRMasterAccessException
        : HRMasterDomainException
    {
        public UnauthorizedHRMasterAccessException(
            string message)
            : base(message, "HR-401")
        {
        }

        public UnauthorizedHRMasterAccessException(
            string message,
            Exception innerException)
            : base(message, "HR-401", innerException)
        {
        }
    }
}