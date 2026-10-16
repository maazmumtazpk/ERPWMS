using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ERPWMS.Application.DTOs.Supplier;
using ERPWMS.Database;
using ERPWMS.Database.Models;


namespace ERPWMS.Application.Queries.Supplier.SearchSuppliers
{
    public class SearchSuppliersQueryHandler : IRequestHandler<SearchSuppliersQuery, SearchSuppliersResult>
    {
        private readonly AppDbContext _context;
        private readonly AutoMapper.IMapper _mapper;

        public SearchSuppliersQueryHandler(
            AppDbContext context,
            AutoMapper.IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<SearchSuppliersResult> Handle(SearchSuppliersQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Suppliers.AsQueryable();

            // Exclude deleted unless explicitly requested
            if (!request.IncludeDeleted)
            {
                query = query.Where(s => s.IsDeleted != true);
            }

            // Apply filters
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.Trim().ToLower();
                query = query.Where(s =>
                    s.SupplierName.ToLower().Contains(searchTerm) ||
                    (s.SupplierCode != null && s.SupplierCode.ToLower().Contains(searchTerm)) ||
                    (s.ContactPerson != null && s.ContactPerson.ToLower().Contains(searchTerm)) ||
                    (s.Phone != null && s.Phone.ToLower().Contains(searchTerm)) ||
                    (s.Email != null && s.Email.ToLower().Contains(searchTerm)) ||
                    (s.City != null && s.City.ToLower().Contains(searchTerm)) ||
                    (s.NTN != null && s.NTN.ToLower().Contains(searchTerm))
                );
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(request.SupplierName))
                {
                    query = query.Where(s => s.SupplierName.Contains(request.SupplierName));
                }

                if (!string.IsNullOrWhiteSpace(request.SupplierCode))
                {
                    query = query.Where(s => s.SupplierCode != null && s.SupplierCode.Contains(request.SupplierCode));
                }

                if (!string.IsNullOrWhiteSpace(request.ContactPerson))
                {
                    query = query.Where(s => s.ContactPerson != null && s.ContactPerson.Contains(request.ContactPerson));
                }

                if (!string.IsNullOrWhiteSpace(request.Phone))
                {
                    query = query.Where(s => s.Phone.Contains(request.Phone));
                }

                if (!string.IsNullOrWhiteSpace(request.Email))
                {
                    query = query.Where(s => s.Email != null && s.Email.Contains(request.Email));
                }

                if (!string.IsNullOrWhiteSpace(request.City))
                {
                    query = query.Where(s => s.City != null && s.City.Contains(request.City));
                }

                if (!string.IsNullOrWhiteSpace(request.NTN))
                {
                    query = query.Where(s => s.NTN != null && s.NTN.Contains(request.NTN));
                }

                if (!string.IsNullOrWhiteSpace(request.TaxId))
                {
                    query = query.Where(s => s.TaxId != null && s.TaxId.Contains(request.TaxId));
                }

                if (request.IsActive.HasValue)
                {
                    query = query.Where(s => s.IsActive == request.IsActive.Value);
                }
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var suppliers = await query
                .OrderByDescending(s => s.SupplierId)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            // Load contact count for each supplier
            foreach (var supplier in suppliers)
            {
                supplier.SupplierContacts = await _context.SupplierContacts
                    .Where(sc => sc.SupplierId == supplier.SupplierId && sc.IsActive == true)
                    .ToListAsync(cancellationToken);
            }

            var result = new SearchSuppliersResult
            {
                Suppliers = _mapper.Map<List<SupplierListDto>>(suppliers),
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };

            return result;
        }
    }
}

