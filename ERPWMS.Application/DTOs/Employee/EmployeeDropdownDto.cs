namespace ERPWMS.Application.DTOs.Employee
{
    public class EmployeeDropdownDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string FullName => $"{EmployeeCode} - {FirstName} {LastName}";
    }
}
