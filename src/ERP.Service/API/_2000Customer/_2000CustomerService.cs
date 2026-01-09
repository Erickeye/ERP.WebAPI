using ERP.EntityModels.Context;
using ERP.EntityModels.Models;
using ERP.Library.Enums;
using ERP.Library.Helpers;
using ERP.Library.ViewModels;
using ERP.Library.ViewModels._1000Company;
using ERP.Library.ViewModels._2000Customer;
using ERP.Library.ViewModels.UserInfo;
using Microsoft.EntityFrameworkCore;

namespace ERP.Service.API._2000Customer
{
    public interface I_2000CustomerService
    {
        Task<ResultModel<ListResult<CustomerListVM>>> Index();
        Task<ResultModel<CustomerInputVM>> Get(int id);
        Task<ResultModel<string>> CreateOrEdit(CustomerInputVM data);
        Task<ResultModel<string>> Delete(int id);
        Task<ResultModel<ListResult<SelectModel>>> GetCustomerSelect();
    }
    public class _2000CustomerService : I_2000CustomerService
    {
        private readonly ERPDbContext _context;

        public _2000CustomerService(ERPDbContext context)
        {
            _context = context;
        }

        public async Task<ResultModel<ListResult<CustomerListVM>>> Index()
        {
            var result = new ResultModel<ListResult<CustomerListVM>>();
            var list = await _context.t_2000Customer
                .Select(x => new CustomerListVM
                {
                    Id = x.Id,
                    Name = x.Name,
                    TaxInvoiceNumber = x.TaxInvoiceNumber,
                    ContactPhone = x.ContactPhone,
                    RegisteredAddress = x.RegisteredAddress
                })
                .ToListAsync();
            return result;
        }
        public async Task<ResultModel<CustomerInputVM>> Get(int id)
        {
            var entity = await _context.t_2000Customer
                .Select(x => new CustomerInputVM
                {
                    Id = id,
                    AttribName = x.AttribName,
                    Name = x.Name,
                    TaxInvoiceNumber = x.TaxInvoiceNumber,
                    ContactPhone = x.ContactPhone,
                    Owner = x.Owner,
                    FaxPhone = x.FaxPhone,
                    StaffChineseName = x.StaffChineseName,
                    RegisteredAddress = x.RegisteredAddress,
                    DeliveryAddress = x.DeliveryAddress,
                    TaxInvoiceAddress = x.TaxInvoiceAddress,
                    BankName = x.BankName,
                    CheckingAccount = x.CheckingAccount,
                    RemittanceAccount = x.RemittanceAccount,
                    PayDays = x.PayDays,
                    CreditLine = x.CreditLine,
                    CreditBalance = x.CreditBalance,
                    LastDeliveryDate = x.LastDeliveryDate,
                    Advance = x.Advance,
                    InvoiceForm = (InvoiceFormType)x.InvoiceForm!,
                })
                .FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }
            return ResultModel.Ok(entity);
        }
        public async Task<ResultModel<string>> CreateOrEdit(CustomerInputVM data)
        {
            var entity = await _context.t_2000Customer.FirstOrDefaultAsync(c => c.Id == data.Id);
            if (entity == null)
            {
                entity = new t_2000Customer();
                ObjectHelper.CopyProperties(data, entity, "Custemploy");
                _context.Add(entity);
                await _context.SaveChangesAsync();
                return ResultModel.Ok("資料成功新增");
            }
            ObjectHelper.CopyProperties(data, entity, "Custemploy");
            await _context.SaveChangesAsync();
            return ResultModel.Ok("資料成功修改");

        }
        public async Task<ResultModel<string>> Delete(int id)
        {
            var entity = _context.t_2000Customer.FirstOrDefault(c => c.Id == id);
            if (entity == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }
            _context.Remove(entity);
            await _context.SaveChangesAsync();
            return ResultModel.Ok("資料已刪除");
        }
        public async Task<ResultModel<ListResult<SelectModel>>> GetCustomerSelect()
        {
            var staff = await _context.t_2000Customer
                .AsNoTracking()
                .Select(x => new SelectModel
                {
                    Value = x.Id,
                    Text = x.Name,
                })
                .ToListAsync();

            return ResultModel.Ok(staff);
        }
    }
}
