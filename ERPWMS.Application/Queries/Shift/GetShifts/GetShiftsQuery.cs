using System;
using System.Collections.Generic;
using MediatR;
using ERPWMS.Application.DTOs.Shift;

namespace ERPWMS.Application.Queries.Shift.GetShifts
{
    public class GetShiftsQuery : IRequest<GetShiftsResult>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool IncludeDeleted { get; set; } = false;
    }

    public class GetShiftsResult
    {
        public List<ShiftListDto> Shifts { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPrevious => Page > 1;
        public bool HasNext => Page < TotalPages;
    }
}