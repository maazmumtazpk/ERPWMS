using MediatR;
using ERPWMS.Application.DTOs.Brand;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Queries.Brand
{
    public class GetBrandsQuery : IRequest<BrandsListDto>
    {
        public string? SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool? IsActive { get; set; }
        public bool IncludeDeleted { get; set; } = false;
    }
}
