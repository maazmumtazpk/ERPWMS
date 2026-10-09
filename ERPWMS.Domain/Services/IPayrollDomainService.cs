using ERPWMS.Database.Models;

namespace ERPWMS.Domain.Services
{
    public interface IPayrollDomainService
    {
        Task<EmployeePayroll> GenerateAsync(
            int employeeId,
            DateOnly monthYear,
            string CreatedByUserId,
            CancellationToken cancellationToken = default);

        Task ApproveAsync(
            int payrollId,
            int approvedByUserId,
            CancellationToken cancellationToken = default);

        Task MarkPaidAsync(
            int payrollId,
            DateTime paymentDate,
            int updatedByUserId,
            CancellationToken cancellationToken = default);
    }
}


