using ERPWMS.Application.Common;
using ERPWMS.Domain.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using ERPWMS.Database.Models;

namespace ERPWMS.Infrastructure.Services
{
    public class DomainEventPublisher : IDomainEventPublisher
    {
        private readonly IMediator _mediator;
        private readonly ILoggingService _logger;

        public DomainEventPublisher(IMediator mediator, ILoggingService logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task PublishAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default) where TEvent : class
        {
            try
            {
                _logger.LogDebug($"Publishing domain event: {typeof(TEvent).Name}");
                await _mediator.Publish(domainEvent, cancellationToken);
                _logger.LogDebug($"Domain event published successfully: {typeof(TEvent).Name}");
            }
            catch (Exception ex)
            {
                // ✅ Log the error but don't break the main flow
                // Domain events should not block the main operation
                _logger.LogError($"Error publishing domain event {typeof(TEvent).Name}. Event: {domainEvent?.ToString() ?? "null"}", ex);

                // ✅ Don't throw - this prevents company registration from failing
                // The company is already saved in the database
            }
        }
    }
}
