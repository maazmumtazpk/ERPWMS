using ERPWMS.Domain.Interfaces;
using ERPWMS.Domain.Services;
using ERPWMS.Domain.Services.DTOs;   // ✅ Domain DTOs
using ERPWMS.Domain.Rules;
using ERPWMS.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ERPWMS.Infrastructure.Services
{
    public class LeaveDomainService(IEmployeeRepository employeeRepository) : ILeaveDomainService
    {
        // =========================================================
        // SUBMIT LEAVE
        // =========================================================
        public async Task<LeaveDetails> SubmitLeaveAsync(
            SubmitLeaveRequest request,
            int userId,
            CancellationToken cancellationToken = default)
        {
            if (request.StartDate > request.EndDate)
                throw new InvalidOperationException("Start date cannot be greater than End date.");

            var employee = await employeeRepository.GetByIdAsync(request.EmployeeId);
            if (employee is null)
                throw new InvalidOperationException("Employee not found.");

            var totalDays = request.EndDate.DayNumber - request.StartDate.DayNumber + 1;

            var existingLeaves = await employeeRepository.GetLeavesByEmployeeAsync(request.EmployeeId);

            var overlap = existingLeaves.Any(x =>
                x.Status != "Rejected" &&
                x.Status != "Cancelled" &&
                request.StartDate <= x.ToDate &&
                request.EndDate >= x.FromDate);

            if (overlap)
                throw new InvalidOperationException("Employee already has a leave request for these dates.");

            var leave = new EmployeeLeave
            {
                EmployeeId = request.EmployeeId,
                LeaveType = request.LeaveTypeId.ToString(),   // ✅ int -> string
                FromDate = request.StartDate,
                ToDate = request.EndDate,
                TotalDays = totalDays,
                Reason = request.Reason?.Trim() ?? string.Empty,
                Status = "Submitted",
                CreatedBy = userId.ToString(),                // ✅ int -> string
                CreatedDate = DateTime.UtcNow
            };

            await employeeRepository.AddLeaveAsync(leave);
            await employeeRepository.SaveAsync();

            return MapToDetails(leave, employee);
        }

        // =========================================================
        // APPROVE LEAVE
        // =========================================================
        public async Task ApproveLeaveAsync(
            int leaveId,
            int approvedByUserId,
            CancellationToken cancellationToken = default)
        {
            var leave = await employeeRepository.GetLeaveByIdAsync(leaveId);
            if (leave is null)
                throw new InvalidOperationException("Leave request not found.");

            if (leave.Status != "Submitted" && leave.Status != "Pending")
                throw new InvalidOperationException("Only submitted or pending leave can be approved.");

            leave.Status = "Approved";
            leave.ApprovedBy = approvedByUserId.ToString();   // ✅ int -> string
            leave.ApprovedDate = DateTime.UtcNow;
            leave.UpdatedBy = approvedByUserId.ToString();   // ✅ int -> string
            leave.UpdatedDate = DateTime.UtcNow;

            await employeeRepository.UpdateLeaveAsync(leave);
            await employeeRepository.SaveAsync();
        }

        // =========================================================
        // REJECT LEAVE
        // =========================================================
        public async Task RejectLeaveAsync(
            int leaveId,
            string rejectionReason,
            int rejectedByUserId,
            CancellationToken cancellationToken = default)
        {
            var leave = await employeeRepository.GetLeaveByIdAsync(leaveId);
            if (leave is null)
                throw new InvalidOperationException("Leave request not found.");

            if (leave.Status != "Submitted" && leave.Status != "Pending")
                throw new InvalidOperationException("Only submitted or pending leave can be rejected.");

            if (string.IsNullOrWhiteSpace(rejectionReason))
                throw new InvalidOperationException("Rejection reason is required.");

            leave.Status = "Rejected";
            leave.RejectionReason = rejectionReason.Trim();
            leave.UpdatedBy = rejectedByUserId.ToString();  // ✅ int -> string
            leave.UpdatedDate = DateTime.UtcNow;

            await employeeRepository.UpdateLeaveAsync(leave);
            await employeeRepository.SaveAsync();
        }

        // =========================================================
        // CANCEL LEAVE
        // =========================================================
        public async Task CancelLeaveAsync(
            int leaveId,
            int userId,
            CancellationToken cancellationToken = default)
        {
            var leave = await employeeRepository.GetLeaveByIdAsync(leaveId);
            if (leave is null)
                throw new InvalidOperationException("Leave request not found.");

            if (leave.Status == "Rejected" || leave.Status == "Cancelled")
                throw new InvalidOperationException("This leave cannot be cancelled.");

            leave.Status = "Cancelled";
            leave.UpdatedBy = userId.ToString();   // ✅ int -> string
            leave.UpdatedDate = DateTime.UtcNow;

            await employeeRepository.UpdateLeaveAsync(leave);
            await employeeRepository.SaveAsync();
        }

        // =========================================================
        // GET LEAVE HISTORY
        // =========================================================
        public async Task<List<LeaveDetails>> GetLeaveHistoryAsync(
            GetLeaveHistoryRequest request,
            CancellationToken cancellationToken = default)
        {
            var employee = await employeeRepository.GetByIdAsync(request.EmployeeId);
            if (employee is null)
                throw new InvalidOperationException("Employee not found.");

            var leaves = await employeeRepository.GetLeavesByEmployeeAsync(request.EmployeeId);

            // ✅ DateTime? -> DateOnly?
            var fromDate = request.FromDate.HasValue
                ? DateOnly.FromDateTime(request.FromDate.Value)
                : (DateOnly?)null;

            var toDate = request.ToDate.HasValue
                ? DateOnly.FromDateTime(request.ToDate.Value)
                : (DateOnly?)null;

            return leaves
                .Where(x =>
                    (!fromDate.HasValue || x.ToDate >= fromDate) &&
                    (!toDate.HasValue || x.FromDate <= toDate) &&
                    (string.IsNullOrWhiteSpace(request.Status) ||
                     string.Equals(x.Status, request.Status, StringComparison.OrdinalIgnoreCase)))
                .OrderByDescending(x => x.FromDate)
                .Select(x => MapToDetails(x, employee))
                .ToList();
        }

        // =========================================================
        // GET LEAVE BALANCE
        // =========================================================
        public async Task<LeaveBalanceDetails> GetLeaveBalanceAsync(
            GetLeaveBalanceRequest request,
            CancellationToken cancellationToken = default)
        {
            var employee = await employeeRepository.GetByIdAsync(request.EmployeeId);
            if (employee is null)
                throw new InvalidOperationException("Employee not found.");

            var leaves = await employeeRepository.GetLeavesByEmployeeAsync(request.EmployeeId);

            var year = request.Year ?? DateTime.UtcNow.Year;
            var start = new DateOnly(year, 1, 1);
            var end = new DateOnly(year, 12, 31);

            var leaveTypeId = request.LeaveTypeId ?? 0;
            var leaveTypeStr = leaveTypeId.ToString();

            var yearlyLeaves = leaves
                .Where(x =>
                    x.LeaveType == leaveTypeStr &&
                    x.FromDate <= end &&
                    x.ToDate >= start)
                .ToList();

            var usedDays = yearlyLeaves
                .Where(x => x.Status == "Approved")
                .Sum(x => x.TotalDays);

            var pendingDays = yearlyLeaves
                .Where(x => x.Status == "Submitted" || x.Status == "Pending")
                .Sum(x => x.TotalDays);

            // ✅ FIX: string pass kiya
            var allowedDays = LeaveRules.GetDefaultAllowedDays(leaveTypeStr);

            return new LeaveBalanceDetails
            {
                EmployeeId = request.EmployeeId,
                LeaveTypeId = leaveTypeId,
                LeaveTypeName = leaveTypeStr,
                TotalEntitled = allowedDays,
                Used = usedDays,
                Pending = pendingDays,
                Remaining = Math.Max(0, allowedDays - usedDays - pendingDays),
                Year = year
            };
        }

        // =========================================================
        // MAP
        // =========================================================
        private static LeaveDetails MapToDetails(EmployeeLeave leave, Employee employee)
        {
            // ✅ string -> int? (safe conversion)
            int? approvedByInt = int.TryParse(leave.ApprovedBy, out var ab) ? ab : (int?)null;

            // ✅ string -> int (safe conversion)
            int leaveTypeIdInt = int.TryParse(leave.LeaveType, out var lt) ? lt : 0;

            return new LeaveDetails
            {
                LeaveId = leave.LeaveId,
                EmployeeId = leave.EmployeeId,
                EmployeeName = $"{employee.FirstName} {employee.LastName}".Trim(),
                LeaveTypeId = leaveTypeIdInt,
                LeaveTypeName = leave.LeaveType,
                StartDate = leave.FromDate,
                EndDate = leave.ToDate,
                TotalDays = leave.TotalDays,
                Reason = leave.Reason,
                Status = leave.Status,
                ApprovedBy = approvedByInt,
                ApprovedDate = leave.ApprovedDate,
                CreatedDate = leave.CreatedDate
            };
        }
    }
}