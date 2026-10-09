using MediatR;

namespace ERPWMS.Application.Commands.Finance.ChartOfAccount
{
    public class DeleteChartOfAccountCommand : IRequest<DeleteChartOfAccountResponse>
    {
        public int AccountId { get; set; }

        // ✅ Will be auto-filled from session
        public int CompanyId { get; set; }

        // Audit
        public int DeletedBy { get; set; }
    }

    public class DeleteChartOfAccountResponse
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
    }
}
