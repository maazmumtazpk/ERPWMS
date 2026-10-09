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


namespace ERPWMS.Application.Commands.Customer.CustomerContact.ChangeCustomerContact
{
    public class ChangeCustomerContactCommandHandler : IRequestHandler<ChangeCustomerContactCommand, CustomerContactDto>
    {
        private readonly AppDbContext _context;
        private readonly AutoMapper.IMapper _mapper;

        public ChangeCustomerContactCommandHandler(
            AppDbContext context,
            AutoMapper.IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CustomerContactDto> Handle(ChangeCustomerContactCommand request, CancellationToken cancellationToken)
        {
            var contact = await _context.CustomerContacts
                .Include(cc => cc.Customer)
                .FirstOrDefaultAsync(cc => cc.ContactId == request.ContactId, cancellationToken);

            if (contact == null)
            {
                throw new CustomerContactNotFoundException(request.ContactId);
            }

            if (contact.Customer == null || contact.Customer.IsDeleted == true)
            {
                throw new CustomerNotFoundException(contact.CustomerId);
            }

            if (contact.Customer.IsActive != true)
            {
                throw new CustomerInactiveException(contact.CustomerId);
            }

            if (contact.IsActive != true)
            {
                throw new CustomerContactInactiveException(request.ContactId);
            }

            // Handle primary contact logic
            if (request.IsPrimary == true && contact.IsPrimary != true)
            {
                var existingPrimary = await _context.CustomerContacts
                    .FirstOrDefaultAsync(cc => cc.CustomerId == contact.CustomerId
                        && cc.IsPrimary == true
                        && cc.IsActive == true
                        && cc.ContactId != request.ContactId, cancellationToken);

                if (existingPrimary != null)
                {
                    existingPrimary.IsPrimary = false;
                }
            }

            contact.ContactName = request.ContactName;
            contact.Phone = request.Phone;
            contact.Mobile = request.Mobile;
            contact.Email = request.Email;
            contact.Department = request.Department;
            contact.IsPrimary = request.IsPrimary ?? false;
            contact.UpdatedBy = request.CurrentUserId.ToString();
            contact.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            await _context.Entry(contact).Reference(cc => cc.CreatedByNavigation).LoadAsync(cancellationToken);
            await _context.Entry(contact).Reference(cc => cc.UpdatedByNavigation).LoadAsync(cancellationToken);
            await _context.Entry(contact).Reference(cc => cc.Customer).LoadAsync(cancellationToken);

            var result = _mapper.Map<CustomerContactDto>(contact);
            return result;
        }
    }
}

