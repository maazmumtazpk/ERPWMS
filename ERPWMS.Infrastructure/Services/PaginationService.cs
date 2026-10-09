
using ERPWMS.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERPWMS.Infrastructure.Services
{
    public class PaginationService : IPaginationService
    {
        public PaginatedResult<T> CreatePaginatedResult<T>(IEnumerable<T> items, int totalCount, int page, int pageSize)
        {
            return new PaginatedResult<T>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public void ValidatePaginationParameters(int page, int pageSize, int maxPageSize = 100)
        {
            if (page < 1)
                throw new ArgumentException("Page must be at least 1.", nameof(page));

            if (pageSize < 1)
                throw new ArgumentException("Page size must be at least 1.", nameof(pageSize));

            if (pageSize > maxPageSize)
                throw new ArgumentException($"Page size cannot exceed {maxPageSize}.", nameof(pageSize));
        }
    }
}