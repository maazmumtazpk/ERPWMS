using ERPWMS.Database.Models;
using ERPWMS.Domain.Interfaces;

namespace ERPWMS.Infrastructure.Services
{
    public class HalfDayCalculator : IHalfDayCalculator
    {
        public bool IsHalfDay(int workingMinutes, int lateMinutes, int halfDayThresholdMinutes)
        {
            // Rule 1: Working less than threshold
            if (workingMinutes > 0 && workingMinutes < halfDayThresholdMinutes)
                return true;

            // Rule 2: Late more than 2 hours
            if (lateMinutes > 120)
                return true;

            return false;
        }
    }
}

