using MediatR;

namespace ERPWMS.Application.Commands.Finance.ChartOfAccount
{
    public class RegisterChartOfAccountCommand : IRequest<RegisterChartOfAccountResponse>
    {
        // ✅ Will be auto-filled from session
        public int CompanyId { get; set; }

        public string AccountCode { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public int? ParentAccountId { get; set; }

        // Audit
        public string CreatedBy { get; set; }
    }

    public class RegisterChartOfAccountResponse
    {
        public bool IsSuccess { get; set; }
        public int AccountId { get; set; }
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
    }
}

