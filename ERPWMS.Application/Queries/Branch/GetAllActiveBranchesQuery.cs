using MediatR;
using ERPWMS.Application.DTOs.Branch;
using System.Collections.Generic;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Queries.Branch
{
    public class GetAllActiveBranchesQuery : IRequest<IEnumerable<RegisterBranchResponse>>
    {
    }
}