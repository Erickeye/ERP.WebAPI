using ERP.EntityModels.Context;
using ERP.Library.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ERP.Service.API._4000Inventory
{
    public interface I_4060SupplierService
    {
        Task<ResultModel<ListResult<SelectModel>>> GetSupplierSelect();
    }
    public class _4060SupplierService : I_4060SupplierService
    {
        private readonly ERPDbContext _db;

        public _4060SupplierService(ERPDbContext db)
        {
            _db = db;
        }

        public async Task<ResultModel<ListResult<SelectModel>>> GetSupplierSelect()
        {
            var data = await _db.t_4060Supplier
                .AsNoTracking()
                .Select(x => new SelectModel
                {
                    Value = x.Id,
                    Text = x.Name ?? string.Empty
                })
                .ToListAsync();

            return ResultModel.Ok(data);
        }
    }
}
