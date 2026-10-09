using System.Collections.Generic;

namespace ERPWMS.Application.Common
{
    
    public interface IPaginationService
    {
      
        PaginatedResult<T> CreatePaginatedResult<T>(IEnumerable<T> items, int totalCount, int page, int pageSize);

       
        void ValidatePaginationParameters(int page, int pageSize, int maxPageSize = 100);
    }

    
    public class PaginatedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPreviousPage => Page > 1;
        public bool HasNextPage => Page < TotalPages;
    }
}