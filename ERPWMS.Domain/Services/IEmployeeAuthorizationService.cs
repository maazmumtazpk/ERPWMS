using System.Threading.Tasks;

namespace ERPWMS.Domain.Services
{
    public interface IEmployeeAuthorizationService
    {
        Task<bool> CanAccessEmployeeAsync(int employeeId, int userId);
        Task<bool> CanManageEmployeeAsync(int employeeId, int userId);
        Task<bool> CanManageAttendanceAsync(int employeeId, int userId);
        Task<bool> CanManagePayrollAsync(int employeeId, int userId);
        Task<bool> CanApproveLeaveAsync(int employeeId, int userId);

        Task EnsureAccessAsync(int employeeId, int userId);
        Task EnsureManagementAsync(int employeeId, int userId);
        Task EnsureAttendanceManagementAsync(int employeeId, int userId);
        Task EnsurePayrollManagementAsync(int employeeId, int userId);
    }
}
