
namespace ERPWMS.Domain.Rules
{
    public static class PayrollRules
    {
        public static readonly string Draft = "Draft";
        public static readonly string Submitted = "Submitted";
        public static readonly string Approved = "Approved";
        public static readonly string Paid = "Paid";
        public static readonly string Cancelled = "Cancelled";

        public static bool CanApprove(string? status)
        {
            return status == Submitted;
        }

        public static bool CanMarkPaid(string? status)
        {
            return status == Approved;
        }

        public static bool IsFinal(string? status)
        {
            return status == Paid ||
                   status == Cancelled;
        }
    }
}

