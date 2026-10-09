using System.Threading;
using System.Threading.Tasks;
using ERPWMS.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using ERPWMS.Database.Models;

namespace ERPWMS.Domain.Events.Handlers.Employee
{
    public class EmployeeRegisteredEventHandler : INotificationHandler<EmployeeRegisteredEvent>
    {
        private readonly ILogger<EmployeeRegisteredEventHandler> _logger;
        public EmployeeRegisteredEventHandler(ILogger<EmployeeRegisteredEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(EmployeeRegisteredEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Employee {EmployeeName} (ID: {EmployeeId}) registered by User {UserId} at {OccurredOn}",
                notification.EmployeeName, notification.EmployeeId, notification.UserId, notification.OccurredOn);
            return Task.CompletedTask;
        }
    }

    public class EmployeeActivatedEventHandler : INotificationHandler<EmployeeActivatedEvent>
    {
        private readonly ILogger<EmployeeActivatedEventHandler> _logger;
        public EmployeeActivatedEventHandler(ILogger<EmployeeActivatedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(EmployeeActivatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Employee {EmployeeId} activated by User {UserId}", notification.EmployeeId, notification.UserId);
            return Task.CompletedTask;
        }
    }

    public class EmployeeDeactivatedEventHandler : INotificationHandler<EmployeeDeactivatedEvent>
    {
        private readonly ILogger<EmployeeDeactivatedEventHandler> _logger;
        public EmployeeDeactivatedEventHandler(ILogger<EmployeeDeactivatedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(EmployeeDeactivatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogWarning("Employee {EmployeeId} deactivated by User {UserId}", notification.EmployeeId, notification.UserId);
            return Task.CompletedTask;
        }
    }

    public class EmployeeDeletedEventHandler : INotificationHandler<EmployeeDeletedEvent>
    {
        private readonly ILogger<EmployeeDeletedEventHandler> _logger;
        public EmployeeDeletedEventHandler(ILogger<EmployeeDeletedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(EmployeeDeletedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogWarning("Employee {EmployeeId} deleted by User {UserId}", notification.EmployeeId, notification.UserId);
            return Task.CompletedTask;
        }
    }

    public class EmployeeShiftAssignedEventHandler : INotificationHandler<EmployeeShiftAssignedEvent>
    {
        private readonly ILogger<EmployeeShiftAssignedEventHandler> _logger;
        public EmployeeShiftAssignedEventHandler(ILogger<EmployeeShiftAssignedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(EmployeeShiftAssignedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Shift {ShiftName} assigned to Employee {EmployeeId} by User {UserId}",
                notification.ShiftName, notification.EmployeeId, notification.UserId);
            return Task.CompletedTask;
        }
    }

    public class AttendanceMarkedEventHandler : INotificationHandler<AttendanceMarkedEvent>
    {
        private readonly ILogger<AttendanceMarkedEventHandler> _logger;
        public AttendanceMarkedEventHandler(ILogger<AttendanceMarkedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(AttendanceMarkedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attendance marked for Employee {EmployeeId} on {Date}: {Status}",
                notification.EmployeeId, notification.Date, notification.Status);
            return Task.CompletedTask;
        }
    }

    public class LeaveApprovedEventHandler : INotificationHandler<LeaveApprovedEvent>
    {
        private readonly ILogger<LeaveApprovedEventHandler> _logger;
        public LeaveApprovedEventHandler(ILogger<LeaveApprovedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(LeaveApprovedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Leave {LeaveId} approved for Employee {EmployeeId} by User {UserId}",
                notification.LeaveId, notification.EmployeeId, notification.UserId);
            return Task.CompletedTask;
        }
    }

    public class LeaveRejectedEventHandler : INotificationHandler<LeaveRejectedEvent>
    {
        private readonly ILogger<LeaveRejectedEventHandler> _logger;
        public LeaveRejectedEventHandler(ILogger<LeaveRejectedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(LeaveRejectedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogWarning("Leave {LeaveId} rejected for Employee {EmployeeId} by User {UserId}. Reason: {Reason}",
                notification.LeaveId, notification.EmployeeId, notification.UserId, notification.Reason);
            return Task.CompletedTask;
        }
    }

    public class PayrollGeneratedEventHandler : INotificationHandler<PayrollGeneratedEvent>
    {
        private readonly ILogger<PayrollGeneratedEventHandler> _logger;
        public PayrollGeneratedEventHandler(ILogger<PayrollGeneratedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(PayrollGeneratedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Payroll generated for Employee {EmployeeId}: Month={MonthYear}, Net={NetPay:C}",
                notification.EmployeeId, notification.MonthYear, notification.NetPay);
            return Task.CompletedTask;
        }
    }

    public class PayrollApprovedEventHandler : INotificationHandler<PayrollApprovedEvent>
    {
        private readonly ILogger<PayrollApprovedEventHandler> _logger;
        public PayrollApprovedEventHandler(ILogger<PayrollApprovedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(PayrollApprovedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Payroll {PayrollId} approved for Employee {EmployeeId} by User {UserId}",
                notification.PayrollId, notification.EmployeeId, notification.UserId);
            return Task.CompletedTask;
        }
    }

    public class PayrollPaidEventHandler : INotificationHandler<PayrollPaidEvent>
    {
        private readonly ILogger<PayrollPaidEventHandler> _logger;
        public PayrollPaidEventHandler(ILogger<PayrollPaidEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(PayrollPaidEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Payroll {PayrollId} paid to Employee {EmployeeId} on {PaymentDate}",
                notification.PayrollId, notification.EmployeeId, notification.PaymentDate);
            return Task.CompletedTask;
        }
    }
}
