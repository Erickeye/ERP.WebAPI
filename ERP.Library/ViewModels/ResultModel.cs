using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.Library.Enums;
using ERP.Library.Extensions;

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
    }

    public class ResultPayload<T>
    {
        public T? Data { get; set; } = default;
        public int Code { get; set; } = 0;
    }
}
