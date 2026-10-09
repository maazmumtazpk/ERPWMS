using MediatR;
using System;
using System.Collections.Generic;

namespace ERPWMS.Application.DTOs.Company
{
    // ============================================================
    // COMPANY DASHBOARD QUERY
    // ============================================================

    public class GetCompanyDashboardQuery
        : IRequest<CompanyDashboardResponse>
    {
    }


    // ============================================================
    // COMPANY DASHBOARD RESPONSE
    // ============================================================

    public class CompanyDashboardResponse
    {
        public int TotalCompanies { get; set; }

        public int ActiveCompanies { get; set; }

        public int InactiveCompanies { get; set; }

        public int AddedThisMonth { get; set; }

        public IEnumerable<CompanySummaryResponse> RecentCompanies { get; set; }
            = new List<CompanySummaryResponse>();
    }
}