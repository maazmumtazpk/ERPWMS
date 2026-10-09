using ERPWMS.Domain.Services.DTOs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Services
{
    public interface ILeaveDomainService
    {
        Task<LeaveDetails> SubmitLeaveAsync(
            SubmitLeaveRequest request,
            int userId,
            CancellationToken cancellationToken = default);

        Task ApproveLeaveAsync(
            int leaveId,
            int approvedByUserId,
            CancellationToken cancellationToken = default);

        Task RejectLeaveAsync(
            int leaveId,
            string rejectionReason,
            int rejectedByUserId,
            CancellationToken cancellationToken = default);

        Task CancelLeaveAsync(
            int leaveId,
            int userId,
            CancellationToken cancellationToken = default);

        Task<List<LeaveDetails>> GetLeaveHistoryAsync(
            GetLeaveHistoryRequest request,
            CancellationToken cancellationToken = default);

        Task<LeaveBalanceDetails> GetLeaveBalanceAsync(
            GetLeaveBalanceRequest request,
            CancellationToken cancellationToken = default);
    }
}