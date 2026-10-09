using MediatR;
using ERPWMS.Application.DTOs.Product;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Queries.Product
{
    public class GetProductByBarcodeQuery : IRequest<ProductDto?>
    {
        public string Barcode { get; set; } = null!;
    }
}
