using MediatR;
using System.Collections.Generic;
using ERPWMS.Application.DTOs.WeeklyOff;

namespace ERPWMS.Application.Queries.WeeklyOff
{
    public class GetWeeklyOffQuery : IRequest<List<WeeklyOffDto>>
    {
        public int? EmployeeId { get; set; }
    }
}
