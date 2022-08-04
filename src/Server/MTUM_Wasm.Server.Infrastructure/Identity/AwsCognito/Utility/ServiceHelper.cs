using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using MTUM_Wasm.Server.Core.Common.Entity;
using MTUM_Wasm.Server.Infrastructure.Identity.AwsCognito.Mapping;
using MTUM_Wasm.Server.Infrastructure.Identity.AwsCognito.Service;
using MTUM_Wasm.Shared.Core.Common.Result;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Infrastructure.Identity.AwsCognito.Utility;

internal class ServiceHelper
{
    public static bool IsSuccessStatusCode(System.Net.HttpStatusCode statusCode)
    {
        return (int)statusCode >= 200 && (int)statusCode <= 299;
    }

    public static async Task<IServiceResult<SystemUserEntity>> FindByEmailAsync(IAmazonCognitoIdentityProvider provider, ICognitoUserPoolWrapper userPool, string normalizedEmail, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = await provider.ListUsersAsync(new ListUsersRequest
        {
            Filter = "email = \"" + normalizedEmail + "\"",
            UserPoolId = userPool.PoolID
        }, cancellationToken);

        if (ServiceHelper.IsSuccessStatusCode(result.HttpStatusCode))
        {
            if (result.Users is null)
                return Result<SystemUserEntity>.Fail($"Cannot find user: '{normalizedEmail}'");

            if (result.Users.Count == 1)
                return Result<SystemUserEntity>.Success(result.Users.Select(p => p.ToSystemUserEntity()).Single());
            else if (result.Users.Count > 1)
                return Result<SystemUserEntity>.Fail($"Multiple users found with: '{normalizedEmail}'");
            else
                return Result<SystemUserEntity>.Fail($"Cannot find user: '{normalizedEmail}'");
        }
        return Result<SystemUserEntity>.Fail($"List users failed with code:{result.HttpStatusCode}");
    }

    /// <summary>
    /// Returns a normalized representation of the specified <paramref name="email"/>.
    /// </summary>
    /// <param name="email">The email to normalize.</param>
    /// <returns>A normalized representation of the specified <paramref name="email"/>.</returns>
    public static string NormalizeEmail(string email)
    {
        return email.Trim().ToLower(CultureInfo.InvariantCulture);
    }
}
