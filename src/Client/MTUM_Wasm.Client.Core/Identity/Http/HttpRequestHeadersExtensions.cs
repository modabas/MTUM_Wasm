using System.Net.Http.Headers;

namespace MTUM_Wasm.Client.Core.Identity.Http;

internal static class HttpRequestHeadersExtensions
{
    public static void Set(this HttpRequestHeaders headers, string name, string? value)
    {
        if (headers.Contains(name)) 
            headers.Remove(name);
        headers.Add(name, value);
    }
}
