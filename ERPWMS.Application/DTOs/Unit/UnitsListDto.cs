using System;
using System.Collections.Generic;

namespace ERPWMS.Application.DTOs.Unit
{
    public class UnitsListDto
    {
        public IEnumerable<UnitDto> Units { get; set; } = new List<UnitDto>();

        public int TotalCount { get; set; }

        public int CurrentPage { get; set; }

        public int PageSize { get; set; }

        public int TotalPages =>
            PageSize > 0
                ? (int)Math.Ceiling((double)TotalCount / PageSize)
                : 0;

        public string? SearchTerm { get; set; }

        public bool? IsActive { get; set; }

        public bool? IsBaseUnit { get; set; }
    }
}