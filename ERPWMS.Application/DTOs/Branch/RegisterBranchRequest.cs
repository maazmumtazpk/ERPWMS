using MediatR;

namespace ERPWMS.Application.DTOs.Branch
{
    /// <summary>
    /// Request to register a new branch
    /// </summary>
    public class RegisterBranchRequest : IRequest<RegisterBranchResponse>
    {
        public string BranchName { get; set; } = string.Empty;
        public string? BranchCode { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public int CompanyId { get; set; }
    }

    /// <summary>
    /// Response for registering a branch
    /// </summary>
    public class RegisterBranchResponse
    {
        public int BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public string? BranchCode { get; set; }
        public int CompanyId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? Message { get; set; }
    }
}