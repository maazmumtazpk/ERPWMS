using MediatR;

namespace ERPWMS.Application.Queries.Unit
{
    public class GetUnitConversionQuery : IRequest<decimal?>
    {
        public int FromUnitId { get; set; }
        public int ToUnitId { get; set; }
        public decimal Quantity { get; set; } = 1;
    }
}
