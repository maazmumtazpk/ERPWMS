using MediatR;

namespace ERPWMS.Application.DTOs.Designation
{
    public class ActivateDesignationCommand : IRequest<DesignationStatusResponse>
    {
        public int DesignationId { get; set; }
    }

    public class DeactivateDesignationCommand : IRequest<DesignationStatusResponse>
    {
        public int DesignationId { get; set; }
        public string? Reason { get; set; }
    }

    public class DesignationStatusResponse
    {
        public int DesignationId { get; set; }
        public string DesignationName { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public int CompanyId { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime OperationDate { get; set; }
        public string? Message { get; set; }
    }
}