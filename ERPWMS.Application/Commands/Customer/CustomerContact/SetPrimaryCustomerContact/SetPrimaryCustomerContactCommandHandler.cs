using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ERPWMS.Domain.Exceptions.TradePartners;
using ERPWMS.Database;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Commands.Customer.CustomerContact.SetPrimaryCustomerContact
{
    public class SetPrimaryCustomerContactCommandHandler : IRequestHandler<SetPrimaryCustomerContactCommand, bool>
    {
        private readonly AppDbContext _context;

        public SetPrimaryCustomerContactCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(SetPrimaryCustomerContactCommand request, CancellationToken cancellationToken)
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

            if (contact.IsPrimary == true)
            {
                return true;
            }

            var existingPrimary = await _context.CustomerContacts
                .FirstOrDefaultAsync(cc => cc.CustomerId == contact.CustomerId
                    && cc.IsPrimary == true
                    && cc.IsActive == true
                    && cc.ContactId != request.ContactId, cancellationToken);

            if (existingPrimary != null)
            {
                existingPrimary.IsPrimary = false;
            }

            contact.IsPrimary = true;
            contact.UpdatedBy = request.CurrentUserId.ToString();
            contact.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
