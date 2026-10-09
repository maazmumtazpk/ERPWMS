using MediatR;

namespace ERPWMS.Application.DTOs.Company
{
    public class ChangeCompanyProfileRequest : IRequest<ChangeCompanyProfileResponse>
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? RegistrationNumber { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? LogoUrl { get; set; }
    }

    public class ChangeCompanyProfileResponse
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public DateTime? UpdatedDate { get; set; }
        public string? Message { get; set; }
    }
}