using MediatR;
using System;
using ERPWMS.Application.DTOs.Holiday;

namespace ERPWMS.Application.Queries.Holiday
{
    public class GetHolidayByDateQuery : IRequest<HolidayDto?>
    {
        public DateOnly Date { get; set; }
        public int? BranchId { get; set; }
    }
}
