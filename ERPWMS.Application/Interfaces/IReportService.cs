using ERPWMS.Application.DTOs.Reports;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Application.Interfaces
{
    public interface IReportService
    {
        Task<List<ReportDefinitionDto>> GetReportsAsync();
        Task<ReportResult> GenerateReportAsync(string code, ReportParameter param, int companyId, string userName);
        byte[] ExportToExcel(ReportResult result);
        byte[] ExportToPdf(ReportResult result);
        string ExportToCsv(ReportResult result);
    }
}
