using MediatR;

namespace ERPWMS.Application.DTOs.Department
{
    /// <summary>
    /// Request to validate department name uniqueness
    /// </summary>
    public class ValidateDepartmentNameRequest : IRequest<ValidationResponse>
    {
        public string DepartmentName { get; set; } = string.Empty;
        public int BranchId { get; set; }
        public int? ExcludingDepartmentId { get; set; }
    }

    /// <summary>
    /// Request to validate department code uniqueness
    /// </summary>
    public class ValidateDepartmentCodeRequest : IRequest<ValidationResponse>
    {
        public string DepartmentCode { get; set; } = string.Empty;
        public int BranchId { get; set; }
        public int? ExcludingDepartmentId { get; set; }
    }

    /// <summary>
    /// Request to validate branch exists and is active
    /// </summary>
    public class ValidateBranchForDepartmentRequest : IRequest<ValidationResponse>
    {
        public int BranchId { get; set; }
    }

    /// <summary>
    /// Request to validate company exists for department
    /// </summary>
    public class ValidateCompanyForDepartmentRequest : IRequest<ValidationResponse>
    {
        public int CompanyId { get; set; }
    }

    /// <summary>
    /// Response for validation operations
    /// </summary>
    public class ValidationResponse
    {
        public bool IsValid { get; set; }
        public string? Message { get; set; }
        public string? ErrorCode { get; set; }
    }
}

