using System;

namespace ERPWMS.Application.DTOs.Employee
{
    public class RegisterEmployeeRequest
    {
        public int DepartmentId { get; set; }
        public int DesignationId { get; set; }
        public int BranchId { get; set; }
        public int? WarehouseId { get; set; }
        public int? ShiftId { get; set; }

        public string EmployeeCode { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? FatherName { get; set; }
        public string? Cnic { get; set; }
        public DateOnly? DOB { get; set; }
        public string? Gender { get; set; }
        public DateOnly HireDate { get; set; }
        public DateOnly? TerminationDate { get; set; }
        public decimal BaseSalary { get; set; }
    }
}

