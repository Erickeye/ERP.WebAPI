using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [SwaggerParameter("頁數，預設第1頁")]
        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = (value > 0) ? value : 1;  // 確保 PageNumber 至少為 1
        }

        private int _pageSize;
        [SwaggerParameter("每頁資料筆數，預設10筆")]
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > 0) ? value : 10;  // 確保 PageSize 至少為 1
        }

        [SwaggerParameter("排序欄位")]
        public string? SortColumn { get; set; }

        [SwaggerParameter("排序方向(預設正序:asc,倒序:desc)")]
        public string SortDirection { get; set; } = "asc";
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
