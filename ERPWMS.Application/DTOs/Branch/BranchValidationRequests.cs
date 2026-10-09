using MediatR;

namespace ERPWMS.Application.DTOs.Branch
{
    /// <summary>
    /// Request to validate branch name uniqueness
    /// </summary>
    public class ValidateBranchNameRequest : IRequest<ValidationResponse>
    {
        public string BranchName { get; set; } = string.Empty;
        public int CompanyId { get; set; }
        public int? ExcludingBranchId { get; set; }
    }

    /// <summary>
    /// Request to validate branch code uniqueness
    /// </summary>
    public class ValidateBranchCodeRequest : IRequest<ValidationResponse>
    {
        public string BranchCode { get; set; } = string.Empty;
        public int CompanyId { get; set; }
        public int? ExcludingBranchId { get; set; }
    }

    /// <summary>
    /// Request to validate company exists and is active
    /// </summary>
    public class ValidateCompanyForBranchRequest : IRequest<ValidationResponse>
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