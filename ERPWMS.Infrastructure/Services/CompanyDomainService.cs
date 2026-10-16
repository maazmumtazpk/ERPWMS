using ERPWMS.Domain.Interfaces;
using ERPWMS.Domain.Rules;
using ERPWMS.Domain.Services;
using ERPWMS.Database.Models;
using ERPWMS.Application.Common;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ERPWMS.Infrastructure.Services
{
    public class CompanyDomainService : ICompanyDomainService
    {
        private readonly ICompanyRepository _repository;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ILoggingService _logger;

        public CompanyDomainService(
            ICompanyRepository repository,
            IDateTimeProvider dateTimeProvider,
            ILoggingService logger)
        {
            _repository = repository;
            _dateTimeProvider = dateTimeProvider;
            _logger = logger;
        }

        public async Task<Company> RegisterCompanyAsync(
            string companyName, string? registrationNumber, string? address,
            string? phone, string? email, string? LogoUrl, string createdBy,
            CancellationToken cancellationToken = default)
        {
            CompanyBusinessRules.ValidateCompanyName(companyName);

            var company = new Company
            {
                CompanyName = companyName,
                RegistrationNo = registrationNumber,
                Address = address,
                Phone = phone,
                Email = email,
                LogoUrl = LogoUrl,
                IsActive = true,
                IsDeleted = false,
                CreatedDate = _dateTimeProvider.Now,
                CreatedBy = createdBy
            };

            await _repository.AddCompanyAsync(company, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
            return company;
        }

        public async Task<Company> UpdateCompanyProfileAsync(
            int companyId, string companyName, string? registrationNumber,
            string? address, string? phone, string? email, string? LogoUrl,
            string updatedBy, CancellationToken cancellationToken = default)
        {
            var company = await GetValidatedCompanyByIdAsync(companyId, cancellationToken);

            company.CompanyName = companyName;
            company.RegistrationNo = registrationNumber;
            company.Address = address;
            company.Phone = phone;
            company.Email = email;
            company.LogoUrl = LogoUrl;
            company.UpdatedDate = _dateTimeProvider.Now;
            company.UpdatedBy = updatedBy;

            _repository.UpdateCompany(company);
            await _repository.SaveChangesAsync(cancellationToken);
            return company;
        }

        public async Task<Company> ActivateCompanyAsync(
            int companyId, string activatedBy,
            CancellationToken cancellationToken = default)
        {
            var company = await GetValidatedCompanyByIdAsync(companyId, cancellationToken);
            company.IsActive = true;
            company.UpdatedDate = _dateTimeProvider.Now;
            company.UpdatedBy = activatedBy;
            _repository.UpdateCompany(company);
            await _repository.SaveChangesAsync(cancellationToken);
            return company;
        }

        public async Task<Company> DeactivateCompanyAsync(
            int companyId, string deactivatedBy,
            CancellationToken cancellationToken = default)
        {
            var company = await GetValidatedCompanyByIdAsync(companyId, cancellationToken);
            company.IsActive = false;
            company.UpdatedDate = _dateTimeProvider.Now;
            company.UpdatedBy = deactivatedBy;
            _repository.UpdateCompany(company);
            await _repository.SaveChangesAsync(cancellationToken);
            return company;
        }

        public async Task ValidateCompanyAsync(
            Company company, CancellationToken cancellationToken = default)
        {
            if (company == null) throw new ArgumentNullException(nameof(company));
            CompanyBusinessRules.ValidateCompanyName(company.CompanyName);
            CompanyBusinessRules.ValidateNotDeleted(company);
            await Task.CompletedTask;
        }

        public async Task<Company> GetValidatedCompanyByIdAsync(
            int companyId, CancellationToken cancellationToken = default)
        {
            var company = await _repository.GetCompanyByIdAsync(companyId, cancellationToken);
            if (company == null)
                throw new Domain.Exceptions.CompanyNotFoundException(companyId);
            CompanyBusinessRules.ValidateNotDeleted(company);
            return company;
        }

        public Task<bool> IsCompanyNameUniqueAsync(string companyName, int? excludeCompanyId = null, CancellationToken cancellationToken = default)
            => _repository.IsCompanyNameUniqueAsync(companyName, excludeCompanyId, cancellationToken);

        public Task<bool> IsRegistrationNumberUniqueAsync(string registrationNumber, int? excludeCompanyId = null, CancellationToken cancellationToken = default)
            => _repository.IsRegistrationNumberUniqueAsync(registrationNumber, excludeCompanyId, cancellationToken);

        public Task<bool> IsEmailUniqueAsync(string email, int? excludeCompanyId = null, CancellationToken cancellationToken = default)
            => string.IsNullOrWhiteSpace(email) ? Task.FromResult(true) : _repository.IsEmailUniqueAsync(email, excludeCompanyId, cancellationToken);

        public Task<bool> IsPhoneUniqueAsync(string phone, int? excludeCompanyId = null, CancellationToken cancellationToken = default)
            => string.IsNullOrWhiteSpace(phone) ? Task.FromResult(true) : _repository.IsPhoneUniqueAsync(phone, excludeCompanyId, cancellationToken);
    }
}
