using System;

namespace ERPWMS.Application.Common
{
    /// <summary>
    /// Provides current date and time
    /// </summary>
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
        DateTime UtcNow { get; }
        DateOnly Today { get; }
    }
}