using System;

namespace ERPWMS.Domain.Services
{
    public class PayrollDomainException
        : Exception
    {
        public PayrollDomainException(
            string message)
            : base(message)
        {
        }
    }
}