using MediatR;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Commands.Customer.CustomerContact.DeactivateCustomerContact
{
    public class DeactivateCustomerContactCommand : IRequest<bool>
    {
        public int ContactId { get; set; }
        public int CurrentUserId { get; set; }
    }
}
