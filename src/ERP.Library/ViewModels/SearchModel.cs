using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace ERP.Library.ViewModels
{
    public class SearchModel
    {
        public SearchModel()
        {
            this.PageNumber = 1;
            this.PageSize = 10;
        }

        private int _pageNumber;
        [SwaggerSchema("頁數，預設第1頁")]
        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = (value > 0) ? value : 1;  // 確保 PageNumber 至少為 1
        }

        private int _pageSize;
        [SwaggerSchema("每頁資料筆數，預設10筆")]
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > 0) ? value : 10;  // 確保 PageSize 至少為 1
        }
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();

        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public int TotalPages =>
            PageSize == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
    }
}
