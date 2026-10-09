using MediatR;

namespace ERPWMS.Application.DTOs.Company
{
    public class ActivateCompanyCommand : IRequest<CompanyStatusResponse>
    {
        public int CompanyId { get; set; }
    }

    
    public class DeactivateCompanyCommand : IRequest<CompanyStatusResponse>
    {
        public int CompanyId { get; set; }
        public string? Reason { get; set; }
    }

   
    public class CompanyStatusResponse
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime OperationDate { get; set; }
        public string? Message { get; set; }
    }
}