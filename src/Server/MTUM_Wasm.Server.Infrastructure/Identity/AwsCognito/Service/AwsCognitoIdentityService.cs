using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using MTUM_Wasm.Server.Core.Identity.Dto;
using MTUM_Wasm.Server.Core.Identity.Service;
using MTUM_Wasm.Server.Infrastructure.Identity.AwsCognito.Utility;
using MTUM_Wasm.Shared.Core.Common.Result;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Infrastructure.Identity.AwsCognito.Service;

internal class AwsCognitoIdentityService : IIdentityService
{
    private readonly IAmazonCognitoIdentityProvider _provider;
    private readonly ICognitoUserPoolWrapper _userPool;
    private readonly ILogger<AwsCognitoIdentityService> _logger;
    private readonly AwsCognitoOptions _options;

    private const string AttributeNameForEmailVerified = "email_verified";

    public AwsCognitoIdentityService(IAmazonCognitoIdentityProvider provider,
        ICognitoUserPoolWrapper userPool,
        ILogger<AwsCognitoIdentityService> logger,
        IOptions<AwsCognitoOptions> awsCognitoOptions)
    {
        _provider = provider;
        _userPool = userPool;
        _logger = logger;
        _options = awsCognitoOptions.Value;
    }

    public async Task<IServiceResult<LoginOutput>> Login(LoginInput requestDto, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var cognitoUser = await GetUserByEmailAsync(ServiceHelper.NormalizeEmail(requestDto.EmailAddress), cancellationToken);
        if (cognitoUser is null)
        {
            return Result<LoginOutput>.Fail($"Unable to retrieve user '{requestDto.EmailAddress}'.");
        }

        var authResponse = await StartValidatePasswordAsync(cognitoUser, requestDto.Password, cancellationToken);

        if (authResponse is not null)
        {
            //logged in
            if (cognitoUser.SessionTokens is not null && cognitoUser.SessionTokens.IsValid())
            {
                var result = authResponse.AuthenticationResult;

                var tokenHandler = new JwtSecurityTokenHandler().ReadJwtToken(result.IdToken);

                var ret = new LoginOutput()
                {
                    Id = Guid.TryParse(tokenHandler.Claims.FirstOrDefault(c => c.Type == "sub")?.Value ?? string.Empty, out var id) ? id : Guid.Empty,
                    AccessToken = result.AccessToken,
                    RefreshToken = result.RefreshToken,
                    RefreshTokenExpiresAt = TryGetRefreshTokenExpirationTime(tokenHandler.Claims.FirstOrDefault(c => c.Type == "iat")?.Value ?? string.Empty, out var refreshTokenExpiryTime) ? refreshTokenExpiryTime : DateTime.MinValue.ToUniversalTime(),
                    IdToken = result.IdToken
                };
                return Result<LoginOutput>.Success(ret);
            }
            else if (authResponse.ChallengeName == ChallengeNameType.NEW_PASSWORD_REQUIRED)
            {
                return Result<LoginOutput>.Fail($"User '{requestDto.EmailAddress}' must change password before signing in.");
            }
        }
        return Result<LoginOutput>.Fail($"Cannot validate password for user '{requestDto.EmailAddress}'.");
    }

    public async Task<IServiceResult<RefreshOutput>> Refresh(RefreshInput requestDto, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var emailAddress = requestDto.Email;
        var cognitoUser = await GetUserByEmailAsync(ServiceHelper.NormalizeEmail(emailAddress), cancellationToken);
        if (cognitoUser is null)
        {
            return Result<RefreshOutput>.Fail($"Unable to retrieve user '{emailAddress}'.");
        }

        cognitoUser.SessionTokens = new CognitoUserSession(string.Empty, string.Empty, requestDto.RefreshToken,
            requestDto.AuthenticatedAt ?? GetRefreshTokenIssuedTime(requestDto.RefreshTokenExpiresAt), requestDto.RefreshTokenExpiresAt);
        var authResponse = await StartValidateRefreshTokenAsync(cognitoUser, cancellationToken);

        //logged in
        if (authResponse is not null && cognitoUser.SessionTokens is not null && cognitoUser.SessionTokens.IsValid())
        {
            var result = authResponse.AuthenticationResult;

            var tokenHandler = new JwtSecurityTokenHandler().ReadJwtToken(result.IdToken);

            var ret = new RefreshOutput()
            {
                Id = Guid.TryParse(tokenHandler.Claims.FirstOrDefault(c => c.Type == "sub")?.Value ?? string.Empty, out var id) ? id : Guid.Empty,
                AccessToken = result.AccessToken,
                RefreshToken = result.RefreshToken,
                RefreshTokenExpiresAt = TryGetRefreshTokenExpirationTime(tokenHandler.Claims.FirstOrDefault(c => c.Type == "iat")?.Value ?? string.Empty, out var refreshTokenExpiryTime) ? refreshTokenExpiryTime : DateTime.MinValue.ToUniversalTime(),
                IdToken = result.IdToken
            };
            return Result<RefreshOutput>.Success(ret);
        }
        return Result<RefreshOutput>.Fail($"Cannot use refresh token for user '{emailAddress}'.");
    }

    public async Task<IServiceResult> Logout(LogoutInput requestDto, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var findUserResult = await ServiceHelper.FindByEmailAsync(_provider, _userPool, ServiceHelper.NormalizeEmail(requestDto.EmailAddress), cancellationToken);
        if (!findUserResult.Succeeded)
        {
            return Result.Fail(findUserResult.Messages);
        }
        if (findUserResult.Data is null)
        {
            return Result.Fail($"Unable to retrieve user '{requestDto.EmailAddress}'.");
        }
        var username = findUserResult.Data.UserName;
        return await GlobalSignOutAsync(username, cancellationToken);
    }

    public async Task<IServiceResult> ChangePassword(ChangePasswordInput requestDto, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var cognitoUser = await GetUserByEmailAsync(ServiceHelper.NormalizeEmail(requestDto.EmailAddress), cancellationToken);
        if (cognitoUser is null)
        {
            return Result.Fail("Failed to change user password. Nake sure email and current password are correct.");
        }

        var ret = await ChangeUserPassword(cognitoUser, requestDto.CurrentPassword, requestDto.NewPassword, cancellationToken);
        return ret;
    }

    public async Task<IServiceResult> ForgotPassword(ForgotPasswordInput requestDto, CancellationToken cancellationToken)
    {
        var cognitoUser = await GetUserByEmailAsync(ServiceHelper.NormalizeEmail(requestDto.EmailAddress), cancellationToken);
        if (cognitoUser is null || !IsEmailVerified(cognitoUser))
        {
            //don't continue flow but don't alert user for errors either
            //like user not found, etc.
            return Result.Success();
        }
        await cognitoUser.ForgotPasswordAsync();
        return Result.Success();
    }

    public async Task<IServiceResult> ResetPassword(ResetPasswordInput requestDto, CancellationToken cancellationToken)
    {
        var cognitoUser = await GetUserByEmailAsync(ServiceHelper.NormalizeEmail(requestDto.EmailAddress), cancellationToken);
        if (cognitoUser is null)
        {
            return Result.Fail("Failed to reset user password. Nake sure email and reset token are correct.");
        }
        try
        {
            await _userPool.ConfirmForgotPassword(cognitoUser.Username, requestDto.ResetToken, requestDto.NewPassword, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Reset password failed.");
            return Result.Fail("Failed to reset user password. Nake sure email and reset token are correct.");
        }
        return Result.Success();
    }

    private bool IsEmailVerified(CognitoUser user)
    {
        if (user.Attributes.TryGetValue(AttributeNameForEmailVerified, out var emailVerifiedString) &&
            bool.TryParse(emailVerifiedString, out var emailVerified))
            return emailVerified;
        return false;
    }

    private bool TryGetRefreshTokenExpirationTime(string tokenIssueTimestamp, out DateTime result)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tokenIssueTimestamp))
            {
                result = DateTime.MinValue.ToUniversalTime();
                return false;
            }
            else
            {
                result = FromUnixTimestamp(long.Parse(tokenIssueTimestamp)).AddMinutes(_options.UserPoolClientRefreshTokenExpirationInMinutes);
                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "TryGetRefreshTokenExpirationTime failed for token issue timestamp: {tokenIssueTimestamp}", tokenIssueTimestamp);
            result = DateTime.MinValue.ToUniversalTime();
            return false;
        }
    }

    private DateTime GetRefreshTokenIssuedTime(DateTime expirationTime)
    {
        return expirationTime.AddMinutes(0 - _options.UserPoolClientRefreshTokenExpirationInMinutes);
    }

    private DateTime FromUnixTimestamp(long timestamp)
    {
        var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return epoch.AddTicks(timestamp * TimeSpan.TicksPerSecond);
    }

    private async Task<CognitoUser?> GetUserByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = await _provider.ListUsersAsync(new ListUsersRequest
        {
            Filter = "email = \"" + normalizedEmail + "\"",
            UserPoolId = _userPool.PoolID
        }, cancellationToken);

        if (result.Users.Count > 0)
        {
            return _userPool.GetUser(result.Users[0].Username,
                result.Users[0].UserStatus,
                result.Users[0].Attributes.ToDictionary(att => att.Name, att => att.Value));
        }
        return null;
    }

    private async Task<IServiceResult> ChangeUserPassword(CognitoUser user, string currentPassword, string newPassword, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        // We start an auth process as the user needs a valid session id to be able to change it's password.
        var authResult = await StartValidatePasswordAsync(user, currentPassword, cancellationToken).ConfigureAwait(false);
        if (authResult != null)
        {
            if (authResult.ChallengeName == ChallengeNameType.NEW_PASSWORD_REQUIRED)
            {
                await user.RespondToNewPasswordRequiredAsync(new RespondToNewPasswordRequiredRequest()
                {
                    SessionID = authResult.SessionID,
                    NewPassword = newPassword
                });
                return Result.Success();
            }
            else if (user.SessionTokens != null && user.SessionTokens.IsValid()) // User is logged in, we can change his password
            {
                await user.ChangePasswordAsync(currentPassword, newPassword);
                return Result.Success();
            }
        }
        return Result.Fail("Failed to change user password. Nake sure email and current password are correct.");
    }

    private async Task<AuthFlowResponse?> StartValidatePasswordAsync(CognitoUser user, string password, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            AuthFlowResponse context =
                await user.StartWithSrpAuthAsync(new InitiateSrpAuthRequest()
                {
                    Password = password
                });

            return context;
        }
        catch (NotAuthorizedException)
        {
            // If the password validation fails then the response flow should be set to null.
            return null;
        }
    }

    private async Task<AuthFlowResponse?> StartValidateRefreshTokenAsync(CognitoUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            AuthFlowResponse context =
                await user.StartWithRefreshTokenAuthAsync(new InitiateRefreshTokenAuthRequest()
                {
                    AuthFlowType = AuthFlowType.REFRESH_TOKEN_AUTH
                });

            return context;
        }
        catch (NotAuthorizedException)
        {
            // If the password validation fails then the response flow should be set to null.
            return null;
        }
    }

    private async Task<IServiceResult> GlobalSignOutAsync(string userName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = await _provider.AdminUserGlobalSignOutAsync(new AdminUserGlobalSignOutRequest()
        {
            UserPoolId = _userPool.PoolID,
            Username = userName
        }, cancellationToken);

        if (ServiceHelper.IsSuccessStatusCode(result.HttpStatusCode))
        {
            return Result.Success();
        }
        else
        {
            return Result.Fail($"Signing out user globally failed with code:{result.HttpStatusCode}");
        }
    }
}

