using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ERPWMS.Application.DTOs.Customer;
using ERPWMS.Domain.Exceptions.TradePartners;
using ERPWMS.Database;
using ERPWMS.Database.Models;


namespace ERPWMS.Application.Commands.Customer.CustomerContact.AddCustomerContact
{
    public class AddCustomerContactCommandHandler : IRequestHandler<AddCustomerContactCommand, CustomerContactDto>
    {
        private readonly AppDbContext _context;
        private readonly AutoMapper.IMapper _mapper;

        public AddCustomerContactCommandHandler(
            AppDbContext context,
            AutoMapper.IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CustomerContactDto> Handle(AddCustomerContactCommand request, CancellationToken cancellationToken)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == request.CustomerId && c.IsDeleted != true, cancellationToken);

            if (customer == null)
            {
                throw new CustomerNotFoundException(request.CustomerId);
            }

            if (customer.IsActive != true)
            {
                throw new CustomerInactiveException(request.CustomerId);
            }

            // Handle primary contact logic
            if (request.IsPrimary == true)
            {
                var existingPrimary = await _context.CustomerContacts
                    .AnyAsync(cc => cc.CustomerId == request.CustomerId
                        && cc.IsPrimary == true
                        && cc.IsActive == true, cancellationToken);

                if (existingPrimary)
                {
                    var currentPrimary = await _context.CustomerContacts
                        .FirstOrDefaultAsync(cc => cc.CustomerId == request.CustomerId
                            && cc.IsPrimary == true
                            && cc.IsActive == true, cancellationToken);

                    if (currentPrimary != null)
                    {
                        currentPrimary.IsPrimary = false;
                    }
                }
            }

            var contact = new ERPWMS.Database.Models.CustomerContact
            {
                CustomerId = request.CustomerId,
                ContactName = request.ContactName,
                Phone = request.Phone,
                Mobile = request.Mobile,
                Email = request.Email,
                Department = request.Department,
                IsPrimary = request.IsPrimary ?? false,
                IsActive = true,
                // ✅ FIX: int → string
                CreatedBy = request.CurrentUserId.ToString(),
                CreatedDate = DateTime.UtcNow
            };

            _context.CustomerContacts.Add(contact);
            await _context.SaveChangesAsync(cancellationToken);

            // ✅ FIX: CreatedByNavigation line HATA di (NotMapped hai)
            // await _context.Entry(contact).Reference(cc => cc.CreatedByNavigation).LoadAsync(cancellationToken);

            await _context.Entry(contact).Reference(cc => cc.Customer).LoadAsync(cancellationToken);

            var result = _mapper.Map<CustomerContactDto>(contact);
            return result;
        }
    }
}