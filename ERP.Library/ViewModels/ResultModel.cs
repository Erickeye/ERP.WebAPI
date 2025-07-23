using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLRIMOA.Library.Enums;
//using TLRIMOA.Library.ViewModels.AnimalManagement.GrassManagement;
//using X.PagedList;

namespace TLRIMOA.Library.ViewModels
{
    /// <summary>
    /// 結果模型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResultModel<T>
    {
        public bool IsSuccess => ErrorCode == ErrorCodeType.None;
        public ErrorCodeType ErrorCode { get; set; } = ErrorCodeType.None;
        public string? ErrorMessage { get; set; }

        public ResultPayload<T> Payload { get; set; } = new();

        public static implicit operator bool(ResultModel<T> vm) => vm.IsSuccess;

        public void SetError(ErrorCodeType code, string? message = null, T? data = default)
        {
            ErrorCode = code;
            ErrorMessage = message ?? code.GetDisplayName();
            Payload.Code = (int)code;
            Payload.Data = data;
            if (message == null) { 

            }
        }

        public void SetSuccess(T data)
        {
            ErrorCode = ErrorCodeType.None;
            Payload.Data = data;
        }
    }

    public class ResultPayload<T>
    {
        public T? Data { get; set; } = default;
        public int Code { get; set; } = 0;
    }

    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            var memberInfo = enumValue.GetType().GetMember(enumValue.ToString());
            if (memberInfo.Length > 0)
            {
                var displayAttr = memberInfo[0].GetCustomAttributes(typeof(DisplayAttribute), false)
                                              .FirstOrDefault() as DisplayAttribute;
                if (displayAttr != null && !string.IsNullOrWhiteSpace(displayAttr.Name))
                {
                    return displayAttr.Name;
                }
            }
            return enumValue.ToString();
        }
    }
}
