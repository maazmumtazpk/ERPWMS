using MediatR;

namespace ERPWMS.Application.DTOs.Finance.ChartOfAccount
{
    public class GetChartOfAccountByIdQuery : IRequest<ChartOfAccountResponse?>
    {
        public int AccountId { get; set; }

        // ✅ Will be auto-filled from session
        public int CompanyId { get; set; }
    }
}
