using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Infrastructure.Identity.AwsCognito.Service;

internal class CognitoUserPoolWrapper : ICognitoUserPoolWrapper
{
    private readonly CognitoUserPool _instance;
    public CognitoUserPoolWrapper(CognitoUserPool userPool)
    {
        _instance = userPool;
    }

    public string PoolID => _instance.PoolID;

    public string ClientID => _instance.ClientID;

    public CognitoUserPool Instance => _instance;

    public Task AdminSignupAsync(string userID, IDictionary<string, string> userAttributes, IDictionary<string, string> validationData)
    {
        return _instance.AdminSignupAsync(userID, userAttributes, validationData);
    }

    public Task ConfirmForgotPassword(string userID, string token, string newPassword, CancellationToken cancellationToken)
    {
        return _instance.ConfirmForgotPassword(userID, token, newPassword, cancellationToken);
    }

    public Task<CognitoUser> FindByIdAsync(string userID)
    {
        return _instance.FindByIdAsync(userID);
    }

    public Task<PasswordPolicyType> GetPasswordPolicyTypeAsync()
    {
        return _instance.GetPasswordPolicyTypeAsync();
    }

    public CognitoUser GetUser()
    {
        return _instance.GetUser();
    }

    public CognitoUser GetUser(string userID)
    {
        return _instance.GetUser(userID);
    }

    public CognitoUser GetUser(string userID, string status, Dictionary<string, string> attributes)
    {
        return _instance.GetUser(userID, status, attributes);
    }

    public Task<CognitoUserPoolClientConfiguration> GetUserPoolClientConfiguration()
    {
        return _instance.GetUserPoolClientConfiguration();
    }

    public Task SignUpAsync(string userID, string password, IDictionary<string, string> userAttributes, IDictionary<string, string> validationData)
    {
        return _instance.SignUpAsync(userID, password, userAttributes, validationData);
    }
}
