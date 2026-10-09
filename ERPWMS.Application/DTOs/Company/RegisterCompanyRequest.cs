using MediatR;

namespace ERPWMS.Application.DTOs.Company
{
    public class RegisterCompanyRequest : IRequest<RegisterCompanyResponse>
    {
        public string CompanyName { get; set; } = string.Empty;
        public string? RegistrationNumber { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? LogoUrl { get; set; }
    }

    public class RegisterCompanyResponse
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? Message { get; set; }
    }
}