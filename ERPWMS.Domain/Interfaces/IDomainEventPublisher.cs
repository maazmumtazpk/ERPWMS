using System.Threading;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Interfaces
{
    
    public interface IDomainEventPublisher
    {
        
        Task PublishAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default) where TEvent : class;
    }
}
