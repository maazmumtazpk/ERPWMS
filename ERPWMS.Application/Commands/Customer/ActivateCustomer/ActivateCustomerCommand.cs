using MediatR;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Commands.Customer.ActivateCustomer
{
    public class ActivateCustomerCommand : IRequest<bool>
    {
        public int CustomerId { get; set; }
        public int CurrentUserId { get; set; }
    }
}
