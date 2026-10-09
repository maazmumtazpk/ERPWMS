using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ERPWMS.Application.DTOs.Customer;
using ERPWMS.Domain.Exceptions.TradePartners;
using ERPWMS.Database;
using ERPWMS.Database.Models;


namespace ERPWMS.Application.Queries.Customer.CustomerContact.GetCustomerContactDetails
{
    public class GetCustomerContactDetailsQueryHandler : IRequestHandler<GetCustomerContactDetailsQuery, CustomerContactDto>
    {
        private readonly AppDbContext _context;
        private readonly AutoMapper.IMapper _mapper;

        public GetCustomerContactDetailsQueryHandler(
            AppDbContext context,
            AutoMapper.IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CustomerContactDto> Handle(GetCustomerContactDetailsQuery request, CancellationToken cancellationToken)
        {
            var contact = await _context.CustomerContacts
                .Include(cc => cc.Customer)
                .FirstOrDefaultAsync(cc => cc.ContactId == request.ContactId, cancellationToken);

            if (contact == null)
            {
                throw new CustomerContactNotFoundException(request.ContactId);
            }

            var result = _mapper.Map<CustomerContactDto>(contact);
            return result;
        }
    }
}
