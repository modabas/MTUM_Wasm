using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Shared.Core.Common.Result
{
    public static class HttpResponseMessageExtensions
    {
        public static async Task<IServiceResult<T>> DeserializeResult<T>(this HttpResponseMessage response, CancellationToken cancellationToken)
        {
            using (var contentStream = await response.Content.ReadAsStreamAsync())
            //close contentStream forcefully if timeout token is cancelled
            using (cancellationToken.Register(() => contentStream?.Close()))
            {
                cancellationToken.ThrowIfCancellationRequested();
                var responseObject = await JsonSerializer.DeserializeAsync<Result<T>>(contentStream, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReferenceHandler = ReferenceHandler.Preserve
                }, cancellationToken: cancellationToken);
                return responseObject ?? Result<T>.Fail("Cannot deserialize Result object from http response message.");
            }
        }

        public static async Task<IServiceResult> DeserializeResult(this HttpResponseMessage response, CancellationToken cancellationToken)
        {
            using (var contentStream = await response.Content.ReadAsStreamAsync())
            //close contentStream forcefully if timeout token is cancelled
            using (cancellationToken.Register(() => contentStream?.Close()))
            {
                cancellationToken.ThrowIfCancellationRequested();
                var responseObject = await JsonSerializer.DeserializeAsync<Result>(contentStream, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReferenceHandler = ReferenceHandler.Preserve
                }, cancellationToken: cancellationToken);
                return responseObject ?? Result.Fail("Cannot deserialize Result object from http response message.");
            }
        }

        //internal static async Task<PaginatedResult<T>> ToPaginatedResult<T>(this HttpResponseMessage response)
        //{
        //    var responseAsString = await response.Content.ReadAsStringAsync();
        //    var responseObject = JsonSerializer.Deserialize<PaginatedResult<T>>(responseAsString, new JsonSerializerOptions
        //    {
        //        PropertyNameCaseInsensitive = true
        //    });
        //    return responseObject;
        //}
    }
}
