using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace ERP.Library.ViewModels._4000Inventory
{
    /// <summary>
    /// 查詢-進貨單搜尋
    /// </summary>
    public class PurchaseSearchVM : SearchModel
    {
        [SwaggerParameter("進貨單號")]
        public string? No { get; set; }

        [SwaggerParameter("供應商識別碼")]
        public string? SupplierName { get; set; }

        [SwaggerParameter("客戶名稱")]
        public string? CustomerName { get; set; }

        [SwaggerParameter("專案名稱")]
        public string? ProjectName { get; set; } = null!;
    }
    /// <summary>
    /// 查詢-進貨單列表
    /// </summary>
    public class PurchaseVM
    {
        [SwaggerParameter("識別碼")]
        public int Id { get; set; }

        [SwaggerParameter("進貨單號")]
        public string? No { get; set; }

        [SwaggerParameter("供應商識別碼")]
        public int SupplierId { get; set; }

        [SwaggerParameter("供應商識別碼")]
        public string? SupplierName { get; set; }

        [SwaggerParameter("客戶識別碼")]
        public int? CustomerId { get; set; }

        [SwaggerParameter("客戶名稱")]
        public string? CustomerName { get; set; }

        [SwaggerParameter("進貨地點識別碼")]
        public int LocationId { get; set; }

        [SwaggerParameter("進貨地點名稱")]
        public string? LocationName { get; set; }

        [SwaggerParameter("專案名稱")]
        public string? ProjectName { get; set; } = null!;

        [SwaggerParameter("購買日期")]
        public DateTime? PurchaseDate { get; set; }

        [SwaggerParameter("是否購買(買或租)")]
        public bool IsPurchase { get; set; } = true;

        [SwaggerParameter("付款方式識別碼")]
        public int PaymentMethodId { get; set; }

        [SwaggerParameter("付款方式")]
        public string? PaymentMethodName { get; set; }

        [SwaggerParameter("價格")]
        public decimal? Amount { get; set; }

        [SwaggerParameter("稅")]
        public decimal? Tax { get; set; }

        [SwaggerParameter("付款人")]
        public string? Payer { get; set; } = null!;

        [SwaggerParameter("發票號碼")]
        public string? InvoiceNumber { get; set; }

        [SwaggerParameter("備註")]
        public string? Note { get; set; }

        [SwaggerParameter("簽核者")]
        public string? Authorizator { get; set; }

        [SwaggerParameter("是否簽核")]
        public bool IsApproval { get; set; }

        [SwaggerParameter("建立日期")]
        public DateTime? CreateTime { get; set; }

        public int CreateUserId { get; set; }
    }
    public class PurchaseAddVM
    {
        [SwaggerParameter("供應商識別碼")]
        public int SupplierId { get; set; }

        [SwaggerParameter("客戶識別碼")]
        public int? CustomerId { get; set; }

        [SwaggerParameter("進貨地點識別碼")]
        public int LocationId { get; set; }

        [SwaggerParameter("專案名稱")]
        public string? ProjectName { get; set; } = null!;

        [SwaggerParameter("購買日期")]
        public DateTime? PurchaseDate { get; set; }

        [SwaggerParameter("是否購買(買或租)")]
        public bool IsPurchase { get; set; } = true;

        [SwaggerParameter("付款方式識別碼")]
        public int PaymentMethodId { get; set; }

        //[SwaggerParameter("價格")]
        //public decimal? Amount { get; set; }

        //[SwaggerParameter("稅")]
        //public decimal? Tax { get; set; }

        [SwaggerParameter("付款人")]
        public string? Payer { get; set; } = null!;

        [SwaggerParameter("發票號碼")]
        public string? InvoiceNumber { get; set; }

        [SwaggerParameter("備註")]
        public string? Note { get; set; }

        public List<PurchaseAddItemVM> Items { get; set; }
            = new List<PurchaseAddItemVM>();
    }
    public class PurchaseAddItemVM
    {

        [SwaggerParameter("種類")]
        public string? Category { get; set; }

         [SwaggerParameter("編號")]
        public string? No { get; set; }

         [SwaggerParameter("名稱")]
        public string? Name { get; set; } = null!;

         [SwaggerParameter("單位")]
        public string? Unit { get; set; }

         [SwaggerParameter("數量")]
        public decimal Quantity { get; set; }

         [SwaggerParameter("價格")]
        public decimal? Price { get; set; }

        // [SwaggerParameter("總計")]
        //public decimal? Total { get; set; }
    }
}
