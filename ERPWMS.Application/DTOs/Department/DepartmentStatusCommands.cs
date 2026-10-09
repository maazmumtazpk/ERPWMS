using MediatR;

namespace ERPWMS.Application.DTOs.Department
{
    /// <summary>
    /// Command to activate a department
    /// </summary>
    public class ActivateDepartmentCommand : IRequest<DepartmentStatusResponse>
    {
        public int DepartmentId { get; set; }
    }

    /// <summary>
    /// Command to deactivate a department
    /// </summary>
    public class DeactivateDepartmentCommand : IRequest<DepartmentStatusResponse>
    {
        public int DepartmentId { get; set; }
        public string? Reason { get; set; }
    }

    /// <summary>
    /// Response for department status change
    /// </summary>
    public class DepartmentStatusResponse
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public int BranchId { get; set; }
        public int CompanyId { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime OperationDate { get; set; }
        public string? Message { get; set; }
    }
}

