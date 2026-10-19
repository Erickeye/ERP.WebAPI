using ERP.EntityModels.Context;
using ERP.EntityModels.Models;
using ERP.Library.Enums._4000Inventory;
using ERP.Library.Enums.Other;
using ERP.Library.Extensions;
using ERP.Library.Helpers;
using ERP.Library.ViewModels;
using ERP.Library.ViewModels._4000Inventory;
using Microsoft.EntityFrameworkCore;

namespace ERP.Service.API._4000Inventory
{
    /// <summary>
    /// 庫存異動紀錄-服務（唯讀）
    /// </summary>
    public interface I_4040StockMovementService
    {
        /// <summary>
        /// 查詢庫存異動紀錄列表
        /// </summary>
        Task<ResultModel<PagedResult<StockMovementVM>>> Index(StockMovementSearchVM vm);
    }

    public class _4040StockMovementService : I_4040StockMovementService
    {
        private readonly ERPDbContext _db;

        public _4040StockMovementService(ERPDbContext db)
        {
            _db = db;
        }

        public async Task<ResultModel<PagedResult<StockMovementVM>>> Index(StockMovementSearchVM vm)
        {
            // 結束時間只有日期（00:00）時，視為包含當天整天
            if (vm.MovementTimeEnd.HasValue && vm.MovementTimeEnd.Value.TimeOfDay == TimeSpan.Zero)
            {
                vm.MovementTimeEnd = vm.MovementTimeEnd.Value.Date.AddDays(1).AddTicks(-1);
            }

            var filter = SearchExpressionBuilder.Build<t_4040StockMovement>(vm);

            // 預設最新的異動在前面
            IQueryable<StockMovementVM> query = _db.t_4040StockMovement
                .AsNoTracking()
                .Where(filter)
                .Select(x => new StockMovementVM
                {
                    Id = x.Id,
                    InventoryId = x.InventoryId,
                    InventoryNo = x.Inventory.No,
                    InventoryName = x.Inventory.Name,
                    LocationName = x.Inventory.Location.Name,
                    MovementType = x.MovementType,
                    SourceTableType = x.SourceTableType,
                    SourceNo = x.SourceNo,
                    Quantity = x.Quantity,
                    BeforeQuantity = x.BeforeQuantity,
                    AfterQuantity = x.AfterQuantity,
                    OperatorUserId = x.OperatorUserId,
                    OperatorName = x.OperatorUser.Name,
                    MovementTime = x.MovementTime
                })
                .OrderByDescending(x => x.Id);

            if (!string.IsNullOrWhiteSpace(vm.SortColumn))
            {
                query = query.ApplySort(vm, SortHelper.GetColumns<StockMovementVM>());
            }

            var pagedResult = await query.ToPagedResultAsync(vm);

            // 顯示名稱由列舉在記憶體端轉換
            foreach (var item in pagedResult.Items)
            {
                item.MovementTypeDisplay = ((StockMovementType)item.MovementType).GetDisplayName();
                item.SourceTableName = ((TableType)item.SourceTableType).GetDisplayName();
            }

            return ResultModel.Ok(pagedResult);
        }
    }
}
