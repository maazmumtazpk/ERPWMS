namespace ERPWMS.Domain.Interfaces
{
    public interface IHalfDayCalculator
    {
        bool IsHalfDay(int workingMinutes, int lateMinutes, int halfDayThresholdMinutes);
    }
}
