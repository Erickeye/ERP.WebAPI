using ERP.Data;
using ERP.EntityModels.Models._2000Customer;
using ERP.Library.Enums;
using ERP.Library.Helpers;
using ERP.Library.ViewModels;
using ERP.Library.ViewModels._1000Company;
using ERP.Library.ViewModels._2000Customer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Service.API._2000Customer
{
    public interface I_2000CustomerService
    {
        Task<ResultModel<ListResult<CustomerListVM>>> Index();
        Task<ResultModel<CustomerInputVM>> Get(int id);
        Task<ResultModel<string>> CreateOrEdit(CustomerInputVM data);
        Task<ResultModel<string>> Delete(int id);
    }
    public class _2000CustomerService : I_2000CustomerService
    {
        private readonly ERPContext _context;

        public _2000CustomerService(ERPContext context)
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
            var result = new ResultModel<CustomerInputVM>();
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
                    InvoiceForm = x.InvoiceForm,
                })
                .FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
            {
                result.SetError(ErrorCodeType.NotFoundData);
                return result;
            }
            result.Data = entity;
            return result;
        }
        public async Task<ResultModel<string>> CreateOrEdit(CustomerInputVM data)
        {
            var result = new ResultModel<string>();
            var entity = await _context.t_2000Customer.FirstOrDefaultAsync(c => c.Id == data.Id);
            if(entity == null)
            {
                entity = new t_2000Customer();
                ObjectHelper.CopyProperties(data, entity, "Custemploy");
                _context.Add(entity);
                result.SetSuccess("資料成功新增");
            }
            else
            {
                ObjectHelper.CopyProperties(data, entity, "Custemploy");
                result.SetSuccess("資料修改新增");
            }
            await _context.SaveChangesAsync();
            return result;
        }
        public async Task<ResultModel<string>> Delete(int id)
        {
            var result = new ResultModel<string>();
            var entity = _context.t_2000Customer.FirstOrDefault(c => c.Id == id);
            if (entity == null)
            {
                result.SetError(ErrorCodeType.NotFoundData);
                return result;
            }
            _context.Remove(entity);
            await _context.SaveChangesAsync();
            result.SetSuccess("資料已刪除");
            return result;
        }
    }
}
