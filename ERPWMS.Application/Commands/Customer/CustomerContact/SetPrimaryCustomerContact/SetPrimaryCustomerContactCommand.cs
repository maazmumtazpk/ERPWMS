using MediatR;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Commands.Customer.CustomerContact.SetPrimaryCustomerContact
{
    public class SetPrimaryCustomerContactCommand : IRequest<bool>
    {
        public int ContactId { get; set; }
        public int CurrentUserId { get; set; }
    }
}
