using MediatR;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Commands.Customer.CustomerContact.ActivateCustomerContact
{
    public class ActivateCustomerContactCommand : IRequest<bool>
    {
        public int ContactId { get; set; }
        public int CurrentUserId { get; set; }
    }
}

