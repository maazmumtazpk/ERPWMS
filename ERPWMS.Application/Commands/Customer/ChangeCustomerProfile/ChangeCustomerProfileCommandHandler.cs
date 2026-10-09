using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ERPWMS.Application.DTOs.Customer;
using ERPWMS.Domain.Exceptions.TradePartners;
using ERPWMS.Database;
using ERPWMS.Database.Models;


namespace ERPWMS.Application.Commands.Customer.ChangeCustomerProfile
{
    public class ChangeCustomerProfileCommandHandler : IRequestHandler<ChangeCustomerProfileCommand, CustomerDto>
    {
        private readonly AppDbContext _context;
        private readonly AutoMapper.IMapper _mapper;

        public ChangeCustomerProfileCommandHandler(
            AppDbContext context,
            AutoMapper.IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CustomerDto> Handle(ChangeCustomerProfileCommand request, CancellationToken cancellationToken)
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

            if (customer.IsDeleted == true)
            {
                throw new CustomerDeletedException(request.CustomerId);
            }

            // Validate customer code uniqueness
            if (!string.IsNullOrWhiteSpace(request.Request.CustomerCode))
            {
                var existingCustomer = await _context.Customers
                    .AnyAsync(c => c.CustomerCode == request.Request.CustomerCode
                        && c.CustomerId != request.CustomerId
                        && c.IsDeleted != true, cancellationToken);

                if (existingCustomer)
                {
                    throw new CustomerCodeAlreadyExistsException(request.Request.CustomerCode);
                }
            }

            // Update customer
            customer.CustomerName = request.Request.CustomerName;
            customer.CustomerCode = request.Request.CustomerCode;
            customer.ContactPerson = request.Request.ContactPerson;
            customer.Phone = request.Request.Phone;
            customer.Mobile = request.Request.Mobile;
            customer.Email = request.Request.Email;
            customer.Website = request.Request.Website;
            customer.Address = request.Request.Address;
            customer.City = request.Request.City;
            customer.State = request.Request.State;
            customer.Country = request.Request.Country;
            customer.PostalCode = request.Request.PostalCode;
            customer.TaxId = request.Request.TaxId;
            customer.NTN = request.Request.NTN;
            customer.CreditLimit = request.Request.CreditLimit;
            customer.CreditDays = request.Request.CreditDays;
            customer.PaymentTerms = request.Request.PaymentTerms;
            customer.UpdatedBy = request.CurrentUserId.ToString();
            customer.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            await _context.Entry(customer).Reference(c => c.CreatedByNavigation).LoadAsync(cancellationToken);
            await _context.Entry(customer).Reference(c => c.UpdatedByNavigation).LoadAsync(cancellationToken);

            var result = _mapper.Map<CustomerDto>(customer);
            return result;
        }
    }
}
