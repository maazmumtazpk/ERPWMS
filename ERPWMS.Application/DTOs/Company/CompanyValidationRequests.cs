using MediatR;

namespace ERPWMS.Application.DTOs.Company
{
    
    public class ValidateCompanyNameRequest : IRequest<ValidationResponse>
    {
        public string CompanyName { get; set; } = string.Empty;
        public int? ExcludingCompanyId { get; set; }
    }

   
    public class ValidateRegistrationNumberRequest : IRequest<ValidationResponse>
    {
        public string RegistrationNumber { get; set; } = string.Empty;
        public int? ExcludingCompanyId { get; set; }
    }

   
    public class ValidationResponse
    {
        public bool IsValid { get; set; }
        public string? Message { get; set; }
        public string? ErrorCode { get; set; }
    }
}