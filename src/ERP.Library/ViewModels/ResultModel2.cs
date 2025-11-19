using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ERP.Library.ViewModels
{
    //public sealed class ErrorMessage
    //{
    //    [JsonPropertyName("code")]
    //    public int Code { get; internal set; }

    //    [JsonPropertyName("data")]
    //    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    //    public object? Data { get; internal set; }

    //    internal ErrorMessage()
    //    {
    //    }
    //}
    //public abstract class ResultBase
    //{
    //    private static readonly Lazy<JsonSerializerOptions> s_jsonSerializerOptions = new Lazy<JsonSerializerOptions>(() => new JsonSerializerOptions
    //    {
    //        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    //    });

    //    private readonly Lazy<string> _json;

    //    [JsonIgnore]
    //    public bool HasError => Error != 0;

    //    [JsonPropertyName("error")]
    //    public int Error { get; protected internal set; }

    //    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    //    [JsonPropertyName("message")]
    //    public object? Message { get; protected internal set; }

    //    private protected ResultBase()
    //    {
    //        _json = new Lazy<string>(() => JsonSerializer.Serialize(this, s_jsonSerializerOptions.Value));
    //    }

    //    public static implicit operator bool(ResultBase result)
    //    {
    //        return !result.HasError;
    //    }

    //    public override string ToString()
    //    {
    //        return _json.Value;
    //    }
    //}
    ///// <summary>
    ///// 結果模型
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    //public sealed class Result : ResultBase
    //{
    //    private static readonly Result s_ok = new Result
    //    {
    //        Error = 0
    //    };

    //    internal Result()
    //    {
    //    }

    //    public static Result Ok()
    //    {
    //        return s_ok;
    //    }

    //    public static Result<TMessage> Ok<TMessage>(TMessage message)
    //    {
    //        return new Result<TMessage>
    //        {
    //            Error = 0,
    //            Message = message
    //        };
    //    }

    //    public static Result<ErrorMessage> Fail(int code)
    //    {
    //        return new Result<ErrorMessage>
    //        {
    //            Error = 1,
    //            Message = new ErrorMessage
    //            {
    //                Code = code
    //            }
    //        };
    //    }

    //    public static Result<ErrorMessage> Fail(int code, object? data)
    //    {
    //        return new Result<ErrorMessage>
    //        {
    //            Error = 1,
    //            Message = new ErrorMessage
    //            {
    //                Code = code,
    //                Data = data
    //            }
    //        };
    //    }
    //}

    //public sealed class Result<TMessage> : ResultBase
    //{
    //    internal Result()
    //    {
    //    }

    //    public static implicit operator Result<TMessage?>(TMessage? message)
    //    {
    //        return Result.Ok(message);
    //    }

    //    public static implicit operator Result<TMessage>(Result<ErrorMessage> result)
    //    {
    //        return new Result<TMessage>
    //        {
    //            Error = result.Error,
    //            Message = result.Message
    //        };
    //    }

    //    public static implicit operator Result(Result<TMessage?> result)
    //    {
    //        return new Result
    //        {
    //            Error = result.Error,
    //            Message = result.Message
    //        };
    //    }

    //    public static implicit operator Result<TMessage?>(Result result)
    //    {
    //        return new Result<TMessage>
    //        {
    //            Error = result.Error,
    //            Message = result.Message
    //        };
    //    }
    //}
}
