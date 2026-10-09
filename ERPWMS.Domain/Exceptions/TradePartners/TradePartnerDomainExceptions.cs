using System;

namespace ERPWMS.Domain.Exceptions.TradePartners
{
    // ============================================================
    // BASE TRADE PARTNER DOMAIN EXCEPTION
    // ============================================================

    public abstract class TradePartnerDomainException : Exception
    {
        public string ErrorCode { get; }

        protected TradePartnerDomainException(
            string message,
            string errorCode)
            : base(message)
        {
            ErrorCode = errorCode;
        }

        protected TradePartnerDomainException(
            string message,
            string errorCode,
            Exception innerException)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
        }
    }


    // ============================================================
    // TRADE PARTNER VALIDATION EXCEPTION
    // ============================================================

    public class TradePartnerValidationException
        : TradePartnerDomainException
    {
        public TradePartnerValidationException(
            string message,
            string errorCode = "TP-099")
            : base(message, errorCode)
        {
        }

        public TradePartnerValidationException(
            string message,
            string errorCode,
            Exception innerException)
            : base(message, errorCode, innerException)
        {
        }
    }


    // ============================================================
    // UNAUTHORIZED TRADE PARTNER ACCESS
    // ============================================================

    public class UnauthorizedTradePartnerAccessException
        : TradePartnerDomainException
    {
        public UnauthorizedTradePartnerAccessException(
            string message)
            : base(message, "TP-401")
        {
        }

        public UnauthorizedTradePartnerAccessException(
            string message,
            Exception innerException)
            : base(message, "TP-401", innerException)
        {
        }
    }
}