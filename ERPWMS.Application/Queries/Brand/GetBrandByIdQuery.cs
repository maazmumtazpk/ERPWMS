using MediatR;
using ERPWMS.Application.DTOs.Brand;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Queries.Brand
{
    public class GetBrandByIdQuery : IRequest<BrandDto?>
    {
        public int BrandId { get; set; }
    }
}