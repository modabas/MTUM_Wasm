using System;

namespace MTUM_Wasm.Server.Core.Common.ExpiringData;

internal class ExpiringData<T> : IExpiringData<T>
{
    public T? Data { get; }

    public DateTime ExpiresAt { get; }

    public ExpiringData(T? data, TimeSpan expiresIn)
    {
        Data = data; ExpiresAt = DateTime.UtcNow.Add(expiresIn);
    }

    public ExpiringData() : this(default, TimeSpan.Zero)
    {
    }

    public bool IsExpired()
    {
        return ExpiresAt <= DateTime.UtcNow;
    }
}
