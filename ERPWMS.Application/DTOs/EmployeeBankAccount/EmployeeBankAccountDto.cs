namespace ERPWMS.Application.DTOs.EmployeeBankAccount
{
    public class EmployeeBankAccountDto
    {
        public int BankAccountId { get; set; }

        public int EmployeeId { get; set; }

        public string BankName { get; set; } = string.Empty;

        public string AccountNumber { get; set; } = string.Empty;

        public string? Iban { get; set; }

        public string? AccountType { get; set; }

        public bool? IsPrimary { get; set; }

        public bool? IsActive { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? UpdatedBy{ get; set; }

        public DateTime? UpdatedDate { get; set; }

        // =====================================================
        // EMPLOYEE INFORMATION
        // =====================================================

        public string? EmployeeName { get; set; }

        public string? EmployeeCode { get; set; }
    }
}
