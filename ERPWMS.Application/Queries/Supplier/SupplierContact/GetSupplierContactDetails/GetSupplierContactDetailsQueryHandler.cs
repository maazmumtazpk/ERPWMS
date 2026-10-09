using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ERPWMS.Application.DTOs.Supplier;
using ERPWMS.Domain.Exceptions.TradePartners;
using ERPWMS.Database;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Queries.Supplier.SupplierContact.GetSupplierContactDetails
{
    public class GetSupplierContactDetailsQueryHandler : IRequestHandler<GetSupplierContactDetailsQuery, SupplierContactDto>
    {
        private readonly AppDbContext _context;
        private readonly AutoMapper.IMapper _mapper;

        public GetSupplierContactDetailsQueryHandler(
            AppDbContext context,
            AutoMapper.IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<SupplierContactDto> Handle(GetSupplierContactDetailsQuery request, CancellationToken cancellationToken)
        {
            var contact = await _context.SupplierContacts
                .Include(sc => sc.Supplier)
                .FirstOrDefaultAsync(sc => sc.ContactId == request.ContactId, cancellationToken);

            if (contact == null)
            {
                throw new SupplierContactNotFoundException(request.ContactId);
            }

            var result = _mapper.Map<SupplierContactDto>(contact);
            return result;
        }
    }
}
