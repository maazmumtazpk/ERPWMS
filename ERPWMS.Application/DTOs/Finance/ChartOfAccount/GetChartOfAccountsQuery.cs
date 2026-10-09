using MediatR;
using System.Collections.Generic;

namespace ERPWMS.Application.DTOs.Finance.ChartOfAccount
{
    public class GetChartOfAccountsQuery : IRequest<List<ChartOfAccountResponse>>
    {
        // ✅ Will be auto-filled from session
        public int CompanyId { get; set; }

        public string? SearchTerm { get; set; }
        public string? AccountType { get; set; }
        public bool? IsActive { get; set; }
        public bool IncludeDeleted { get; set; } = false;
    }
}
