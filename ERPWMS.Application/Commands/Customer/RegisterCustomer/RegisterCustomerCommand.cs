using MediatR;
using ERPWMS.Application.DTOs.Customer;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Commands.Customer.RegisterCustomer
{
    public class RegisterCustomerCommand : IRequest<CustomerDto>
    {
        public RegisterCustomerRequest Request { get; set; } = null!;
        public int CurrentUserId { get; set; }
    }
}
