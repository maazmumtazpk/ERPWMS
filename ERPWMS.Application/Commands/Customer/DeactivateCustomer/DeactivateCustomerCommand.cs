using MediatR;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Commands.Customer.DeactivateCustomer
{
    public class DeactivateCustomerCommand : IRequest<bool>
    {
        public int CustomerId { get; set; }
        public int CurrentUserId { get; set; }
    }
}
