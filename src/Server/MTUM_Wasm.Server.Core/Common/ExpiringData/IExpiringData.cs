using System;

namespace MTUM_Wasm.Server.Core.Common.ExpiringData;

internal interface IExpiringData<T>
{
    public T? Data { get; }
    public DateTime ExpiresAt { get; }
    public bool IsExpired();
}
