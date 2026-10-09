using System;

namespace ERPWMS.Application.Common
{
    /// <summary>
    /// Application logging service interface
    /// </summary>
    public interface ILoggingService
    {
        void LogInformation(string message);
        void LogWarning(string message);
        void LogError(string message, Exception? exception = null);
        void LogDebug(string message);
        void LogCritical(string message, Exception? exception = null);
    }
}