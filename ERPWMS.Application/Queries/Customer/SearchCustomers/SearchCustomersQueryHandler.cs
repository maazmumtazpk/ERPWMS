using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ERPWMS.Application.DTOs.Customer;
using ERPWMS.Database;
using ERPWMS.Database.Models;


namespace ERPWMS.Application.Queries.Customer.SearchCustomers
{
    public class SearchCustomersQueryHandler : IRequestHandler<SearchCustomersQuery, SearchCustomersResult>
    {
        private readonly AppDbContext _context;
        private readonly AutoMapper.IMapper _mapper;

        public SearchCustomersQueryHandler(
            AppDbContext context,
            AutoMapper.IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<SearchCustomersResult> Handle(SearchCustomersQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Customers.AsQueryable();

            if (!request.IncludeDeleted)
            {
                query = query.Where(c => c.IsDeleted != true);
            }

            // Apply search/filters
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.Trim().ToLower();
                query = query.Where(c =>
                    c.CustomerName.ToLower().Contains(searchTerm) ||
                    (c.CustomerCode != null && c.CustomerCode.ToLower().Contains(searchTerm)) ||
                    (c.ContactPerson != null && c.ContactPerson.ToLower().Contains(searchTerm)) ||
                    (c.Phone != null && c.Phone.ToLower().Contains(searchTerm)) ||
                    (c.Email != null && c.Email.ToLower().Contains(searchTerm)) ||
                    (c.City != null && c.City.ToLower().Contains(searchTerm)) ||
                    (c.NTN != null && c.NTN.ToLower().Contains(searchTerm))
                );
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(request.CustomerName))
                {
                    query = query.Where(c => c.CustomerName.Contains(request.CustomerName));
                }

                if (!string.IsNullOrWhiteSpace(request.CustomerCode))
                {
                    query = query.Where(c => c.CustomerCode != null && c.CustomerCode.Contains(request.CustomerCode));
                }

                if (!string.IsNullOrWhiteSpace(request.ContactPerson))
                {
                    query = query.Where(c => c.ContactPerson != null && c.ContactPerson.Contains(request.ContactPerson));
                }

                if (!string.IsNullOrWhiteSpace(request.Phone))
                {
                    query = query.Where(c => c.Phone.Contains(request.Phone));
                }

                if (!string.IsNullOrWhiteSpace(request.Email))
                {
                    query = query.Where(c => c.Email != null && c.Email.Contains(request.Email));
                }

                if (!string.IsNullOrWhiteSpace(request.City))
                {
                    query = query.Where(c => c.City != null && c.City.Contains(request.City));
                }

                if (!string.IsNullOrWhiteSpace(request.NTN))
                {
                    query = query.Where(c => c.NTN != null && c.NTN.Contains(request.NTN));
                }

                if (!string.IsNullOrWhiteSpace(request.TaxId))
                {
                    query = query.Where(c => c.TaxId != null && c.TaxId.Contains(request.TaxId));
                }

                if (request.IsActive.HasValue)
                {
                    query = query.Where(c => c.IsActive == request.IsActive.Value);
                }
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var customers = await query
                .OrderByDescending(c => c.CustomerId)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            foreach (var customer in customers)
            {
                customer.CustomerContacts = await _context.CustomerContacts
                    .Where(cc => cc.CustomerId == customer.CustomerId && cc.IsActive == true)
                    .ToListAsync(cancellationToken);
            }

            var result = new SearchCustomersResult
            {
                Customers = _mapper.Map<List<CustomerListDto>>(customers),
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };

            return result;
        }
    }
}
