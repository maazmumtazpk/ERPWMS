using MediatR;
using ERPWMS.Application.DTOs.Customer;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Commands.Customer.CustomerContact.AddCustomerContact
{
    public class AddCustomerContactCommand : IRequest<CustomerContactDto>
    {
        public int CustomerId { get; set; }
        public string ContactName { get; set; } = null!;
        public string? Phone { get; set; }
        public string Mobile { get; set; } = null!;
        public string? Email { get; set; }
        public string? Department { get; set; }
        public bool? IsPrimary { get; set; }
        public int CurrentUserId { get; set; }
    }
}
