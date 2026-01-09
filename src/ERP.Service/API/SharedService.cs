using ERP.EntityModels.Context;
using ERP.Library.ViewModels;
using Microsoft.EntityFrameworkCore;


namespace ERP.Service.API
{
    public interface ISharedService
    {
        Task<ResultModel<ListResult<SelectModel>>> GetInventorySelect();
        Task<ResultModel<ListResult<SelectModel>>> GetPaymentMethodSelect();
    }
    public class SharedService : ISharedService
    {
       private readonly ERPDbContext _db;

        public SharedService(ERPDbContext db)
        {
            _db = db;
        }

        public async Task<ResultModel<ListResult<SelectModel>>> GetInventorySelect()
        {
            var data = await _db.SystemConfig
                .AsNoTracking()
                .Where(x => x.ConfigType == "InventoryLocation")
                .Select(x => new SelectModel
                {
                    Value = x.Id,
                    Text = x.Name
                })
                .ToListAsync();

            return ResultModel.Ok(data);
        }
        public async Task<ResultModel<ListResult<SelectModel>>> GetPaymentMethodSelect()
        {
            var data = await _db.SystemConfig
                .AsNoTracking()
                .Where(x => x.ConfigType == "PaymentMethod")
                .Select(x => new SelectModel
                {
                    Value = x.Id,
                    Text = x.Name
                })
                .ToListAsync();

            return ResultModel.Ok(data);
        }
    }
}
