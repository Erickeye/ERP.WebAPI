using ERP.Data;
using ERP.EntityModels.Models._1000Company;
using ERP.Library.Enums;
using ERP.Library.ViewModels;
using ERP.Library.ViewModels._1000Company;
using ERP.Library.ViewModels.UserInfo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Net.Mime.MediaTypeNames;

namespace ERP.Service.API._1000Company
{
    public interface I_1000Service
    {
        Task<ResultModel<ListResult<StaffIndex>>> GetStaffIndex(string deptID, bool isResignation);
        Task<ResultModel<string>> CreateOrEdit(t_1000Staff data);
        Task<ResultModel<string>> Delete(int id);
        Task<ResultModel<string>> uploadImg(uploadImg data);
        Task<ResultModel<string>> UploadCertificate(UploadCertificate data);
        Task<ResultModel<string>> EditCertificate(EditCertificate data);
        Task<ResultModel<string>> DeleteCertificate(int id);
        Task<ResultModel<string>> GetCertificate(int id);
    }
    public class _1000Service : I_1000Service
    {
        private readonly ERPContext _context;

        public _1000Service(ERPContext context)
        {
            _context = context;
        }

        public async Task<ResultModel<ListResult<StaffIndex>>> GetStaffIndex(string? deptID, bool isResignation) { 
            var result = new ResultModel<ListResult<StaffIndex>>();

            var _1000 = _context.t_1000Staff.ToList();

            IQueryable<StaffIndex> query;

            //未離職員工
            if (!isResignation)
            {
                if (!string.IsNullOrEmpty(deptID))
                {
                    // Inner Join
                    query = _context.t_1000Staff!
                        .Join(
                            _context.t_1101DepartmentUnit!.Where(d => d.DepartmentId == deptID),
                            staff => staff.StaffId,
                            dept => dept.StaffId,
                            (staff, dept) => staff
                        )
                        .AsNoTracking()
                        .Select(StaffSelector());
                }
                else
                {
                    // Left Join + where dept == null
                    query = _context.t_1000Staff!
                        .GroupJoin(
                            _context.t_1101DepartmentUnit!,
                            staff => staff.StaffId,
                            dept => dept.StaffId,
                            (staff, deptGroup) => new { staff, deptGroup }
                        )
                        .SelectMany(
                            x => x.deptGroup.DefaultIfEmpty(),
                            (x, dept) => new { x.staff, dept }
                        )
                        .Where(x => x.dept == null)
                        .AsNoTracking()
                        .Select(x => x.staff)
                        .Select(StaffSelector());
                }
            }
            else
            {
                // 取得已離職員工
                query = _context.t_1000Staff!
                    .Where(q => q.ResignationDate.HasValue)
                    .AsNoTracking()
                    .Select(StaffSelector());
            }
            if (query == null)
            {
                result.SetError(0, "找不到資料");
                return result;
            }
            var viewModel = await query.ToListAsync();
            var listResult = new ListResult<StaffIndex> { Items = viewModel };
            result.Data = listResult;

            return result;
        }

        public async Task<ResultModel<string>> CreateOrEdit(t_1000Staff data)
        {
            var result = new ResultModel<string>();

            var hasData = _context.t_1000Staff.FirstOrDefault(c => c.StaffId == data.StaffId);
            if (hasData == null) {
                _context.Add(data);
                result.SetSuccess("資料成功新增");
            }
            else
            {
                _context.Entry(hasData).CurrentValues.SetValues(data);
                result.SetSuccess("資料成功修改");
            }
            await _context.SaveChangesAsync();
            return result;
        }
        public async Task<ResultModel<string>> Delete(int id)
        {
            var result = new ResultModel<string>();
            var hasData = _context.t_1000Staff.FirstOrDefault(c => c.StaffId == id);
            if (hasData == null)
            {
                result.SetError(ErrorCodeType.NotFoundData);
                return result;
            }
            _context.Remove(hasData);
            await _context.SaveChangesAsync();
            result.SetSuccess("資料已刪除");
            return result;
        }
        public async Task<ResultModel<string>> uploadImg(uploadImg data)
        {
            var result = new ResultModel<string>();

            const long maxSize = 2 * 1024 * 1024; // 最大 2MB

            // 檢查圖片是否存在
            if (data.image == null || data.image.Length == 0)
            {
                result.SetError(ErrorCodeType.ImgNotFound);
                return result;
            }
            // 檢查圖片大小
            if (data.image.Length > maxSize)
            {
                result.SetError(ErrorCodeType.ImgOver2MB);
                return result;
            }
            // 檢查圖片格式
            var allowedContentTypes = new[] { "image/jpeg", "image/png", "image/gif" }; // 可接受的圖片格式
            if (!allowedContentTypes.Contains(data.image.ContentType))
            {
                result.SetError(ErrorCodeType.ImgNotSupport);
                return result;
            }
            //查詢是否有此使用者
            var staff = _context.t_1000Staff.Find(data.StaffId);
            if (staff == null)
            {
                result.SetError(ErrorCodeType.NotFoundData, "找不到該使用者");
            }
            try
            {
                // 將圖片轉換為 Byte 陣列
                using (var memoryStream = new MemoryStream())
                {
                    using var ms = new MemoryStream();
                    await data.image.CopyToAsync(ms);
                    byte[] imageBytes = ms.ToArray();
                    staff!.Headshot = imageBytes;
                    await _context.SaveChangesAsync();

                    return result;
                }
            }
            catch (Exception ex)
            {
                // 錯誤處理
                result.SetError(ErrorCodeType.ImgNotFound, $"上傳過程中發生錯誤：{ex.Message}");
                return result;
            }
        }
        public async Task<ResultModel<string>> GetCertificate(int id)
        {
            var result = new ResultModel<string>();
            var cert = await _context.t_1001StaffCertificates!.FirstOrDefaultAsync(c => c.Id == id);
            if (cert?.Certificate == null)
            {
                result.SetError(ErrorCodeType.NotFoundData);
                return result;
                
            }
            string base64 = Convert.ToBase64String(cert.Certificate);
            string imageDataUrl = $"data:image/jpeg;base64,{base64}";
            result.Data = imageDataUrl;
            return result;
        }
        public async Task<ResultModel<string>> UploadCertificate(UploadCertificate data)
        {
            var result = new ResultModel<string>();

            if (data.CertificateFile != null && data.CertificateFile.Length > 0)
            {
                using var ms = new MemoryStream();
                await data.CertificateFile.CopyToAsync(ms);
                var cert = new t_1001StaffCertificates
                {
                    StaffId = data.StaffId,
                    CertificateName =  data.CertificateName,
                    CertificateDate =  data.CertificateDate,
                    EffectiveDate =  data.EffectiveDate,
                    Certificate =  ms.ToArray(),
                };

                _context.t_1001StaffCertificates!.Add(cert);
                await _context.SaveChangesAsync();
                result.SetSuccess("證照已成功上傳");
            }

            return result;
        }
        public async Task<ResultModel<string>> EditCertificate(EditCertificate data)
        {
            var result = new ResultModel<string>();
            var _1001 = await  _context.t_1001StaffCertificates!.FindAsync(data.Id);
            if (_1001 != null)
            {
                _1001.CertificateName = data.CertificateName;
                _1001.CertificateDate = data.CertificateDate;
                _1001.EffectiveDate = data.EffectiveDate;
            }
            await _context.SaveChangesAsync();
            result.SetSuccess("證照已成功修改");
            return result;
        }
        public async Task<ResultModel<string>> DeleteCertificate(int id)
        {
            var result = new ResultModel<string>();
            var _1001 = await _context.t_1001StaffCertificates!.FindAsync(id);
            if (_1001 == null)
            {
                result.SetError(ErrorCodeType.NotFoundData);
                return result;
            }
            _context.Remove(_1001);
            await _context.SaveChangesAsync();
            result.SetSuccess("證照已成功刪除");
            return result;
        }

        private static Expression<Func<t_1000Staff, StaffIndex>> StaffSelector()
        {
            return staff => new StaffIndex
            {
                StaffUid = staff.StaffUid,
                Name = staff.ChineseName,
                IdCard = staff.IdCard,
                Gender = staff.Gender.ToString(),
                Bitrthday = staff.Birthday,
                ContactPhone = staff.ContactPhone,
                LineId = staff.LineId,
                Email = staff.Email,
                ContactAddress = staff.ContactAddress
            };
        }
    }
}
