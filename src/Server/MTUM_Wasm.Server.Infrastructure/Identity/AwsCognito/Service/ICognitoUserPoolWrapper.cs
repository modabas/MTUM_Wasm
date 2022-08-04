using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Infrastructure.Identity.AwsCognito.Service;

internal interface ICognitoUserPoolWrapper
{
    public string PoolID { get; }
    public string ClientID { get; }

    public CognitoUserPool Instance { get; }

    public Task AdminSignupAsync(string userID, IDictionary<string, string> userAttributes, IDictionary<string, string> validationData);
    public Task ConfirmForgotPassword(string userID, string token, string newPassword, CancellationToken cancellationToken);
    public Task<CognitoUser> FindByIdAsync(string userID);
    public Task<PasswordPolicyType> GetPasswordPolicyTypeAsync();
    public CognitoUser GetUser();
    public CognitoUser GetUser(string userID);
    public CognitoUser GetUser(string userID, string status, Dictionary<string, string> attributes);
    public Task<CognitoUserPoolClientConfiguration> GetUserPoolClientConfiguration();
    public Task SignUpAsync(string userID, string password, IDictionary<string, string> userAttributes, IDictionary<string, string> validationData);
}
