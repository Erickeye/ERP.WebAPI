using ERP.EntityModels.Context;
using ERP.EntityModels.Models;
using ERP.Library.Enums;
using ERP.Library.Helpers;
using ERP.Library.ViewModels;
using ERP.Library.ViewModels._1000Company;
using ERP.Library.ViewModels.UserInfo;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ERP.Service.API._1000Company
{
    public interface I_1000Service
    {
        Task<ResultModel<ListResult<StaffListVM>>> GetStaffIndex(string deptID, bool isResignation);
        Task<ResultModel<string>> CreateOrEdit(StaffInputVM data);
        Task<ResultModel<string>> Delete(int id);
        Task<ResultModel<string>> uploadImg(UploadImg data);
        Task<ResultModel<string>> UploadCertificate(UploadCertificate data);
        Task<ResultModel<string>> EditCertificate(EditCertificate data);
        Task<ResultModel<string>> DeleteCertificate(int id);
        Task<ResultModel<string>> GetCertificate(int id);
    }
    public class _1000Service : I_1000Service
    {
        private readonly ERPDbContext _context;

        public _1000Service(ERPDbContext context)
        {
            _context = context;
        }

        public async Task<ResultModel<ListResult<StaffListVM>>> GetStaffIndex(string? deptID, bool isResignation) { 
            var _1000 = _context.t_1000Staff.ToList();

            IQueryable<StaffListVM> query;

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
                return ResultModel<ListResult<StaffListVM>>.Error (ErrorCodeType.NotFoundData);
            }
            var viewModel = await query.ToListAsync();
            return ResultModel.Ok(viewModel);

        }

        public async Task<ResultModel<string>> CreateOrEdit(StaffInputVM data)
        {
            var entity = _context.t_1000Staff.FirstOrDefault(c => c.StaffId == data.StaffId);
            if (entity == null) {
                entity = new t_1000Staff();
                ObjectHelper.CopyProperties(data, entity, "StaffCertificates");
                _context.Add(entity);
                await _context.SaveChangesAsync();
                return ResultModel.Ok("資料成功新增");
            }
            else
            {
                ObjectHelper.CopyProperties(data, entity, "StaffCertificates");
                await _context.SaveChangesAsync();
                return ResultModel.Ok("資料成功修改");
            }
        }
        public async Task<ResultModel<string>> Delete(int id)
        {
            var entity = _context.t_1000Staff.FirstOrDefault(c => c.StaffId == id);
            if (entity == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }
            _context.Remove(entity);
            await _context.SaveChangesAsync();
            return ResultModel.Ok("資料已刪除");
        }
        public async Task<ResultModel<string>> uploadImg(UploadImg data)
        {
            const long maxSize = 2 * 1024 * 1024; // 最大 2MB

            // 檢查圖片是否存在
            if (data.image == null || data.image.Length == 0)
            {
                return ResultModel.Error(ErrorCodeType.ImgNotFound);
            }
            // 檢查圖片大小
            if (data.image.Length > maxSize)
            {
                return ResultModel.Error(ErrorCodeType.ImgOver2MB);
            }
            // 檢查圖片格式
            var allowedContentTypes = new[] { "image/jpeg", "image/png", "image/gif" }; // 可接受的圖片格式
            if (!allowedContentTypes.Contains(data.image.ContentType))
            {
                return ResultModel.Error(ErrorCodeType.ImgNotSupport);
            }
            //查詢是否有此使用者
            var staff = _context.t_1000Staff.Find(data.StaffId);
            if (staff == null)
            {
                return ResultModel.Error(ErrorCodeType.ImgNotSupport, "找不到該使用者");
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

                    return ResultModel.Ok();
                }
            }
            catch (Exception ex)
            {
                // 錯誤處理
                return ResultModel.Error(ErrorCodeType.ImgNotFound, $"上傳過程中發生錯誤：{ex.Message}");
            }
        }
        public async Task<ResultModel<string>> GetCertificate(int id)
        {
            var cert = await _context.t_1001StaffCertificates!.FirstOrDefaultAsync(c => c.Id == id);
            if (cert?.Certificate == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }
            string base64 = Convert.ToBase64String(cert.Certificate);
            string imageDataUrl = $"data:image/jpeg;base64,{base64}";
            return ResultModel.Ok(imageDataUrl);
        }
        public async Task<ResultModel<string>> UploadCertificate(UploadCertificate data)
        {
            if (data.CertificateFile == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }
            using var ms = new MemoryStream();
            await data.CertificateFile.CopyToAsync(ms);
            var cert = new t_1001StaffCertificates
            {
                StaffId = data.StaffId,
                CertificateName = data.CertificateName,
                CertificateDate = data.CertificateDate,
                EffectiveDate = data.EffectiveDate,
                Certificate = ms.ToArray(),
            };

            _context.t_1001StaffCertificates!.Add(cert);
            await _context.SaveChangesAsync();
            return ResultModel.Ok("證照已成功上傳");            
        }
        public async Task<ResultModel<string>> EditCertificate(EditCertificate data)
        {
            var _1001 = await  _context.t_1001StaffCertificates!.FindAsync(data.Id);
            if (_1001 != null)
            {
                _1001.CertificateName = data.CertificateName;
                _1001.CertificateDate = data.CertificateDate;
                _1001.EffectiveDate = data.EffectiveDate;
            }
            await _context.SaveChangesAsync();
            return ResultModel.Ok("證照已成功修改");
        }
        public async Task<ResultModel<string>> DeleteCertificate(int id)
        {
            var _1001 = await _context.t_1001StaffCertificates!.FindAsync(id);
            if (_1001 == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }
            _context.Remove(_1001);
            await _context.SaveChangesAsync();
            return ResultModel.Ok("證照已成功刪除");
        }

        private static Expression<Func<t_1000Staff, StaffListVM>> StaffSelector()
        {
            return staff => new StaffListVM
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
