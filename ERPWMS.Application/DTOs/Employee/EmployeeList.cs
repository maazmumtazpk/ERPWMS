using System;

namespace ERPWMS.Application.DTOs.Employee
{
    public class EmployeeList
    {
        public int EmployeeId { get; set; }

        public string EmployeeCode { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string DepartmentName { get; set; } = string.Empty;

        public string DesignationName { get; set; } = string.Empty;

        public string BranchName { get; set; } = string.Empty;

        public string WarehouseName { get; set; } = string.Empty;

        public bool? IsActive { get; set; }

        public DateOnly HireDate { get; set; }

        public DateOnly? TerminationDate { get; set; }
    }
}