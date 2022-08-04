using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTUM_Wasm.Shared.Core.Common.Result;

[Serializable]
public class Result : IServiceResult
{
    public Result()
    {
    }

    public List<string> Messages { get; set; } = new List<string>();

    public bool Succeeded { get; set; }

    public static IServiceResult Fail()
    {
        return new Result { Succeeded = false };
    }

    public static IServiceResult Fail(string message)
    {
        return new Result { Succeeded = false, Messages = new List<string> { message } };
    }

    public static IServiceResult Fail(List<string> messages)
    {
        return new Result { Succeeded = false, Messages = messages };
    }

    public static IServiceResult Success()
    {
        return new Result { Succeeded = true };
    }

    public static IServiceResult Success(string message)
    {
        return new Result { Succeeded = true, Messages = new List<string> { message } };
    }
}

[Serializable]
public class Result<T> : Result, IServiceResult<T>
{
    public Result()
    {
    }

    public T? Data { get; set; }

    public IServiceResult ToBaseResult()
    {
        return new Result() { Messages = this.Messages, Succeeded = this.Succeeded };
    }

    public new static IServiceResult<T> Fail()
    {
        return new Result<T> { Succeeded = false };
    }

    public new static IServiceResult<T> Fail(string message)
    {
        return new Result<T> { Succeeded = false, Messages = new List<string> { message } };
    }

    public new static IServiceResult<T> Fail(List<string> messages)
    {
        return new Result<T> { Succeeded = false, Messages = messages };
    }

    public new static IServiceResult<T> Success()
    {
        return new Result<T> { Succeeded = true };
    }

    public new static IServiceResult<T> Success(string message)
    {
        return new Result<T> { Succeeded = true, Messages = new List<string> { message } };
    }

    public static IServiceResult<T> Success(T? data)
    {
        return new Result<T> { Succeeded = true, Data = data };
    }

    public static IServiceResult<T> Success(T? data, string message)
    {
        return new Result<T> { Succeeded = true, Data = data, Messages = new List<string> { message } };
    }

    public static IServiceResult<T> Success(T? data, List<string> messages)
    {
        return new Result<T> { Succeeded = true, Data = data, Messages = messages };
    }
}
