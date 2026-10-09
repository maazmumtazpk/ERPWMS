using System;
using MediatR;

namespace ERPWMS.Application.DTOs.Department
{
    /// <summary>
    /// Request to register a new department.
    /// CompanyId is intentionally not included because
    /// it is derived from the selected BranchId.
    /// </summary>
    public class RegisterDepartmentRequest : IRequest<RegisterDepartmentResponse>
    {
        public string DepartmentName { get; set; } = string.Empty;

        public string? DepartmentCode { get; set; }

        public int BranchId { get; set; }
    }

    /// <summary>
    /// Response for registering a department.
    /// CompanyId is returned because it is derived from the Branch.
    /// </summary>
    public class RegisterDepartmentResponse
    {
        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; } = string.Empty;

        public string? DepartmentCode { get; set; }

        public int BranchId { get; set; }

        public int CompanyId { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }

        public string? Message { get; set; }
    }
}