using MediatR;

namespace ERPWMS.Application.Commands.SubCategory
{
    public class DeleteSubCategoryCommand : IRequest<bool>
    {
        public int SubCategoryId { get; set; }
        public int DeletedBy { get; set; }
    }
}
