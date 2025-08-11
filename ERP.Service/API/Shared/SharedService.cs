using ERP.Data;
using ERP.Library.Enums._1000Company;
using ERP.Library.ViewModels;
using ERP.Service.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Service.API.Shared
{
    public interface ISharedService
    {
        ResultModel<List<SelectModel>> GetEnumList<TEnum>() where TEnum : Enum;
        //ResultModel<List<SelectModel>> GetBloodType();
        //ResultModel<List<SelectModel>> GetMarriageStatus();
        //ResultModel<List<SelectModel>> GetGender();
        //ResultModel<List<SelectModel>> GetJobStatus();
        //ResultModel<List<SelectModel>> GetLeaveType();
        //ResultModel<List<SelectModel>> GetOverTimeType();
        //ResultModel<List<SelectModel>> GetDocumentLevelType();
    }
    public class SharedService : ISharedService
    {

        public ResultModel<List<SelectModel>> GetEnumList<TEnum>() where TEnum : Enum
        {
            var result = new ResultModel<List<SelectModel>>();
            var list = EnumHelper.ToSelectList<TEnum>();
            result.SetSuccess(list);
            return result;
        }

        //public ResultModel<List<SelectModel>> GetBloodType()
        //{
        //    var result = new ResultModel<List<SelectModel>>();
        //    var list = EnumHelper.ToSelectList<BloodType>();
        //    result.SetSuccess(list);
        //    return result;
        //}
        //public ResultModel<List<SelectModel>> GetMarriageStatus()
        //{
        //    var result = new ResultModel<List<SelectModel>>();
        //    var list = EnumHelper.ToSelectList<MarriageStatus>();
        //    result.SetSuccess(list);
        //    return result;
        //}
        //public ResultModel<List<SelectModel>> GetGender()
        //{
        //    var result = new ResultModel<List<SelectModel>>();
        //    var list = EnumHelper.ToSelectList<Gender>();
        //    result.SetSuccess(list);
        //    return result;
        //}
        //public ResultModel<List<SelectModel>> GetJobStatus()
        //{
        //    var result = new ResultModel<List<SelectModel>>();
        //    var list = EnumHelper.ToSelectList<JobStatus>();
        //    result.SetSuccess(list);
        //    return result;
        //}
        //public ResultModel<List<SelectModel>> GetLeaveType()
        //{
        //    var result = new ResultModel<List<SelectModel>>();
        //    var list = EnumHelper.ToSelectList<LeaveType>();
        //    result.SetSuccess(list);
        //    return result;
        //}
        //public ResultModel<List<SelectModel>> GetOverTimeType()
        //{
        //    var result = new ResultModel<List<SelectModel>>();
        //    var list = EnumHelper.ToSelectList<OverTimeType>();
        //    result.SetSuccess(list);
        //    return result;
        //}

        //public ResultModel<List<SelectModel>> GetDocumentLevelType()
        //{
        //    var result = new ResultModel<List<SelectModel>>();
        //    var list = EnumHelper.ToSelectList<DocumentLevelType>();
        //    result.SetSuccess(list);
        //    return result;
        //}
    }
}
