using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.Library.Enums;

namespace ERP.Library.ViewModels
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

        public T? Data { get; set; }

        public static implicit operator bool(ResultModel<T> vm) => vm.IsSuccess;

        public void SetError(ErrorCodeType code, string? message = null, T? data = default)
        {
            ErrorCode = code;
            ErrorMessage = message ?? code.GetDisplayName();
            Data = data;
        }

        public void SetSuccess(T data)
        {
            ErrorCode = ErrorCodeType.None;
            Data = data;
        }
    }
    public class ListResult<T>
    {
        public List<T> Items { get; set; } = new();
        public ListResult() { }

        public ListResult(List<T> items)
        {
            Items = items ?? new();
        }
    }
    public class SelectModel
    {
        public int Value { get; set; }
        public string Text { get; set; } = string.Empty;
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
