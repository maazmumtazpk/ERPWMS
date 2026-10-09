using System.Threading.Tasks;

namespace ERPWMS.Domain.Interfaces
{
    public interface ILatePolicyEngine
    {
        Task<LatePolicyResult> EvaluateAsync(int employeeId, int year, int month);
    }

    public class LatePolicyResult
    {
        public int EmployeeId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }

        public int TotalLateDays { get; set; }
        public int TotalLateMinutes { get; set; }
        public int TotalEarlyDays { get; set; }
        public int TotalEarlyMinutes { get; set; }

        public int LateHalfDays { get; set; }
        public int LateFullDays { get; set; }
        public int EarlyHalfDays { get; set; }

        public decimal TotalDeductionDays { get; set; }
        public string WarningLevel { get; set; } = "None";
        public string Summary { get; set; } = string.Empty;
    }
}
