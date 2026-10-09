using MediatR;

namespace ERPWMS.Application.Commands.Finance.ChartOfAccount
{
    public class UpdateChartOfAccountCommand : IRequest<UpdateChartOfAccountResponse>
    {
        public int AccountId { get; set; }

        // ✅ Will be auto-filled from session
        public int CompanyId { get; set; }

        public string AccountCode { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public int? ParentAccountId { get; set; }
        public bool? IsActive { get; set; }

        // Audit
        public int UpdatedBy { get; set; }
    }

    public class UpdateChartOfAccountResponse
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
    }
}
