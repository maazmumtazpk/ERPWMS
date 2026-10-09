using MediatR;

namespace ERPWMS.Application.Commands.SubCategory
{
    public class ToggleSubCategoryStatusCommand : IRequest<bool>
    {
        public int SubCategoryId { get; set; }
        public bool IsActive { get; set; }
        public int UpdatedBy { get; set; }
    }
}
