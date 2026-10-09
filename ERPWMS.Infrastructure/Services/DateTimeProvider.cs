
using ERPWMS.Application.Common;
using System;

namespace ERPWMS.Infrastructure.Services
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Now => DateTime.Now;
        public DateTime UtcNow => DateTime.UtcNow;
        public DateOnly Today => DateOnly.FromDateTime(DateTime.Today);
    }
}