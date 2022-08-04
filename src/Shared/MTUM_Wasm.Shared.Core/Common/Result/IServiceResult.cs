using System.Collections.Generic;

namespace MTUM_Wasm.Shared.Core.Common.Result;

public interface IServiceResult
{
    List<string> Messages { get; set; }

    bool Succeeded { get; set; }
}

public interface IServiceResult<out T> : IServiceResult
{
    T? Data { get; }

    IServiceResult ToBaseResult();
}
