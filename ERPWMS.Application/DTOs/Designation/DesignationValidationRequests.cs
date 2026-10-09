using MediatR;

namespace ERPWMS.Application.DTOs.Designation
{
    public class ValidateDesignationNameRequest : IRequest<ValidationResponse>
    {
        public string DesignationName { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public int? ExcludingDesignationId { get; set; }
    }

    public class ValidateDepartmentForDesignationRequest : IRequest<ValidationResponse>
    {
        public int DepartmentId { get; set; }
    }

    public class ValidateCompanyForDesignationRequest : IRequest<ValidationResponse>
    {
        public int CompanyId { get; set; }
    }

    public class ValidationResponse
    {
        public bool IsValid { get; set; }
        public string? Message { get; set; }
        public string? ErrorCode { get; set; }
    }
}