using MediatR;
using ERPWMS.Application.DTOs.Branch;
using ERPWMS.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Queries.Branch
{
    public class GetAllActiveBranchesQueryHandler : IRequestHandler<GetAllActiveBranchesQuery, IEnumerable<RegisterBranchResponse>>
    {
        private readonly IBranchRepository _branchRepository;

        public GetAllActiveBranchesQueryHandler(IBranchRepository branchRepository)
        {
            _branchRepository = branchRepository;
        }

        public async Task<IEnumerable<RegisterBranchResponse>> Handle(GetAllActiveBranchesQuery request, CancellationToken cancellationToken)
        {
            var branches = await _branchRepository.GetAllActiveBranchesAsync(cancellationToken);

            return branches.Select(b => new RegisterBranchResponse
            {
                BranchId = b.BranchId,
                BranchName = b.BranchName,
                BranchCode = b.BranchCode,
                CompanyId = b.CompanyId,
                IsActive = b.IsActive ?? false,
                CreatedDate = b.CreatedDate ?? DateTime.UtcNow
            });
        }
    }
}
