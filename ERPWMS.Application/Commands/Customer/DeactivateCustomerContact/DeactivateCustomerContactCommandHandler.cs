using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ERPWMS.Domain.Exceptions.TradePartners;
using ERPWMS.Database;
using ERPWMS.Database.Models;


namespace ERPWMS.Application.Commands.Customer.CustomerContact.DeactivateCustomerContact
{
    public class DeactivateCustomerContactCommandHandler : IRequestHandler<DeactivateCustomerContactCommand, bool>
    {
        private readonly AppDbContext _context;

        public DeactivateCustomerContactCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeactivateCustomerContactCommand request, CancellationToken cancellationToken)
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
                return true;
            }

            // Check if this is the only primary contact
            if (contact.IsPrimary == true)
            {
                var otherActiveContacts = await _context.CustomerContacts
                    .CountAsync(cc => cc.CustomerId == contact.CustomerId
                        && cc.IsActive == true
                        && cc.ContactId != request.ContactId, cancellationToken);

                if (otherActiveContacts == 0)
                {
                    throw CustomerContactPrimaryException.CustomerRequiresPrimaryContact(contact.CustomerId);
                }
            }

            contact.IsActive = false;
            contact.UpdatedBy = request.CurrentUserId.ToString();
            contact.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
