using MediatR;

namespace ERPWMS.Application.DTOs.Branch
{
    /// <summary>
    /// Command to activate a branch
    /// </summary>
    public class ActivateBranchCommand : IRequest<BranchStatusResponse>
    {
        public int BranchId { get; set; }
    }

    /// <summary>
    /// Command to deactivate a branch
    /// </summary>
    public class DeactivateBranchCommand : IRequest<BranchStatusResponse>
    {
        public int BranchId { get; set; }
        public string? Reason { get; set; }
    }

    /// <summary>
    /// Response for branch status change
    /// </summary>
    public class BranchStatusResponse
    {
        public int BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public int CompanyId { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime OperationDate { get; set; }
        public string? Message { get; set; }
    }
}