using ERP.EntityModels.Context;
using ERP.EntityModels.Models;
using Microsoft.EntityFrameworkCore;

namespace ERP.Service.API
{
    public interface ISerialService
    {
        Task<string> GenerateAsync(string prefix);
    }
    public class SerialService : ISerialService
    {
        private readonly ERPDbContext _db;
        public SerialService(ERPDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// prefix + ROC 年兩位數 + 月兩位數 + 三位流水號
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> GenerateAsync(string prefix)
        {
            var now = DateTime.Now;
            var rocYear = now.Year - 1911;
            var dateKey = $"{rocYear:D2}{now.Month:D2}";

            for (int retry = 0; retry < 3; retry++)
            {
                using var tx = await _db.Database.BeginTransactionAsync();

                var serial = await _db.SerialNumber
                    .FirstOrDefaultAsync(x => x.Prefix == prefix && x.DateKey == dateKey);

                try
                {
                    if (serial == null)
                    {
                        serial = new SerialNumber
                        {
                            Prefix = prefix,
                            DateKey = dateKey,
                            CurrentNo = 1
                        };
                        _db.SerialNumber.Add(serial);
                    }
                    else
                    {
                        serial.CurrentNo += 1;
                    }

                    await _db.SaveChangesAsync();
                    await tx.CommitAsync();

                    return $"{prefix}{dateKey}{serial.CurrentNo:D3}";
                }
                catch (DbUpdateException)
                {
                    // 撞 UNIQUE KEY，重試
                    await tx.RollbackAsync();
                }
            }

            throw new Exception("多次嘗試後序號仍產出失敗。");
        }
    }
}
