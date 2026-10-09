using MediatR;
using ERPWMS.Application.DTOs.Customer;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Commands.Customer.ChangeCustomerProfile
{
    public class ChangeCustomerProfileCommand : IRequest<CustomerDto>
    {
        public int CustomerId { get; set; }
        public RegisterCustomerRequest Request { get; set; } = null!;
        public int CurrentUserId { get; set; }
    }
}
