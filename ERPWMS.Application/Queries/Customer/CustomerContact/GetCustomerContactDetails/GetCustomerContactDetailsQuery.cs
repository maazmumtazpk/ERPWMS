using MediatR;
using ERPWMS.Application.DTOs.Customer;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Queries.Customer.CustomerContact.GetCustomerContactDetails
{
    public class GetCustomerContactDetailsQuery : IRequest<CustomerContactDto>
    {
        public int ContactId { get; set; }
    }
}
