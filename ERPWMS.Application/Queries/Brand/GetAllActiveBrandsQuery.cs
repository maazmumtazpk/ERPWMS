using MediatR;
using ERPWMS.Application.DTOs.Brand;
using System.Collections.Generic;
using ERPWMS.Database.Models;
namespace ERPWMS.Application.Queries.Brand 
{
    public class GetAllActiveBrandsQuery : IRequest<IEnumerable<BrandDto>>
    { 
        public bool OnlyActive { get; set; } = true; }
}