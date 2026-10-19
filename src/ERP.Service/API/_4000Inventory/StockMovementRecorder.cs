using ERP.EntityModels.Context;
using ERP.EntityModels.Models;
using ERP.Library.Enums._4000Inventory;
using ERP.Library.Enums.Other;

namespace ERP.Service.API._4000Inventory
{
    /// <summary>
    /// 庫存異動寫入元件（領料單與進貨單共用）
    /// </summary>
    public interface IStockMovementRecorder
    {
        /// <summary>
        /// 調整庫存項目數量並新增一筆異動紀錄。
        /// 只會變更追蹤中的實體，不呼叫 SaveChanges，由呼叫端統一儲存，
        /// 讓「庫存數量」與「異動紀錄」永遠在同一次儲存內成功或失敗。
        /// </summary>
        /// <param name="inventory">庫存項目（可為尚未儲存的新項目）</param>
        /// <param name="movementType">異動類型</param>
        /// <param name="sourceTableType">來源單據類型</param>
        /// <param name="sourceNo">來源單號</param>
        /// <param name="quantityChange">異動數量，增加為正、減少為負</param>
        /// <param name="operatorUserId">操作人（最後核准人或作廢人）</param>
        /// <param name="movementTime">異動時間（核准或作廢當下）</param>
        void Apply(
            t_4000Inventory inventory,
            StockMovementType movementType,
            TableType sourceTableType,
            string sourceNo,
            decimal quantityChange,
            int operatorUserId,
            DateTime movementTime);
    }

    public class StockMovementRecorder : IStockMovementRecorder
    {
        private readonly ERPDbContext _db;

        public StockMovementRecorder(ERPDbContext db)
        {
            _db = db;
        }

        public void Apply(
            t_4000Inventory inventory,
            StockMovementType movementType,
            TableType sourceTableType,
            string sourceNo,
            decimal quantityChange,
            int operatorUserId,
            DateTime movementTime)
        {
            // 庫存數量欄位可為 null，視為 0
            var before = inventory.Quantity ?? 0m;
            var after = before + quantityChange;

            inventory.Quantity = after;

            // 以導覽屬性關聯，庫存項目為新增時 FK 會在儲存時自動帶入
            _db.t_4040StockMovement.Add(new t_4040StockMovement
            {
                Inventory = inventory,
                MovementType = (int)movementType,
                SourceTableType = (int)sourceTableType,
                SourceNo = sourceNo,
                Quantity = quantityChange,
                BeforeQuantity = before,
                AfterQuantity = after,
                OperatorUserId = operatorUserId,
                MovementTime = movementTime
            });
        }
    }
}
