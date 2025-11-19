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
    public readonly struct ErrorResult
    {
        public ErrorCodeType Code { get; }
        public string? Message { get; }

        public ErrorResult(ErrorCodeType code, string? message = null)
        {
            Code = code;
            Message = message;
        }
    }

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

        public static ResultModel<T> Ok(T data) => new ResultModel<T>
        {
            ErrorCode = ErrorCodeType.None,
            Data = data
        };

        public static ResultModel<T> Error(ErrorCodeType code, string? message = null, T? data = default)
        => new ResultModel<T>
        {
            ErrorCode = code,
            ErrorMessage = message ?? code.GetDisplayName(),
            Data = data
        };

        public static implicit operator ResultModel<T>(ErrorResult result)
        {
            return Error(result.Code, result.Message);
        }


    }
    public class ResultModel
    {
        public static ResultModel<ListResult<T>> Ok<T>(List<T> items)
        {
            var listResult = new ListResult<T>(items);
            return ResultModel<ListResult<T>>.Ok(listResult);
        }

        public static ResultModel<T> Ok<T>(T data) =>
            ResultModel<T>.Ok(data);

        public static ResultModel<string> Ok() =>
        ResultModel<string>.Ok("Success");

        public static ErrorResult Error(ErrorCodeType code, string? message = null)
        => new ErrorResult(code, message);
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
}
