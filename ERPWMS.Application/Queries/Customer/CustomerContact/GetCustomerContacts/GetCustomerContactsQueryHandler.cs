using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ERPWMS.Application.DTOs.Customer;
using ERPWMS.Domain.Exceptions.TradePartners;
using ERPWMS.Database;
using ERPWMS.Database.Models;


namespace ERPWMS.Application.Queries.Customer.CustomerContact.GetCustomerContacts
{
    public class GetCustomerContactsQueryHandler : IRequestHandler<GetCustomerContactsQuery, GetCustomerContactsResult>
    {
        private readonly AppDbContext _context;
        private readonly AutoMapper.IMapper _mapper;

        public GetCustomerContactsQueryHandler(
            AppDbContext context,
            AutoMapper.IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<GetCustomerContactsResult> Handle(GetCustomerContactsQuery request, CancellationToken cancellationToken)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == request.CustomerId && c.IsDeleted != true, cancellationToken);

            if (customer == null)
            {
                throw new CustomerNotFoundException(request.CustomerId);
            }

            var query = _context.CustomerContacts
                .Where(cc => cc.CustomerId == request.CustomerId)
                .AsQueryable();

            if (!request.IncludeInactive)
            {
                query = query.Where(cc => cc.IsActive == true);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var contacts = await query
                .OrderByDescending(cc => cc.IsPrimary)
                .ThenBy(cc => cc.ContactName)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            foreach (var contact in contacts)
            {
                contact.Customer = customer;
            }

            var result = new GetCustomerContactsResult
            {
                Contacts = _mapper.Map<List<CustomerContactDto>>(contacts),
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };

            return result;
        }
    }
}
