using MTUM_Wasm.Client.Core.Utility.MessageDisplay;
using MTUM_Wasm.Shared.Core.Identity.Service;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Client.Core.Identity.Service;

internal class RefreshingAuthenticationStateProvider : AuthenticationStateProvider, IDisposable
{
    private readonly ITokenClaimResolver _tokenClaimResolver;
    private readonly ITokenStore _tokenStore;
    private readonly IIdentityService _identityService;
    private readonly IMessageDisplayService _messageDisplay;

    private CancellationTokenSource _loopCancellationTokenSource = new();
    private readonly TimeSpan _revalidationInterval = TimeSpan.FromMinutes(1);

    public ClaimsPrincipal? AuthenticationStateUser { get; set; }

    public RefreshingAuthenticationStateProvider(ITokenClaimResolver tokenClaimResolver, ITokenStore tokenStore, IIdentityService identityService, IMessageDisplayService messageDisplay)
    {
        _tokenClaimResolver = tokenClaimResolver;
        _tokenStore = tokenStore;

        _identityService = identityService;
        _messageDisplay = messageDisplay;
        // Whenever we receive notification of a new authentication state, cancel any
        // existing revalidation loop and start a new one
        AuthenticationStateChanged += authenticationStateTask =>
        {
            _loopCancellationTokenSource?.Cancel();
            _loopCancellationTokenSource = new CancellationTokenSource();
            _ = RevalidationLoop(authenticationStateTask, _loopCancellationTokenSource.Token);
        };

    }

    public async Task StateChangedAsync()
    {
        var authState = Task.FromResult(await GetAuthenticationStateAsync());

        NotifyAuthenticationStateChanged(authState);

    }

    public void MarkUserAsLoggedOut()
    {
        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
        var authState = Task.FromResult(new AuthenticationState(anonymousUser));

        NotifyAuthenticationStateChanged(authState);
    }

    public async Task<ClaimsPrincipal> GetAuthenticationStateProviderUserAsync()
    {
        var state = await GetAuthenticationStateAsync();
        var authenticationStateProviderUser = state.User;
        return authenticationStateProviderUser;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var accessToken = await _tokenStore.GetTokenProperty<string>(TokenPropertyNameEnum.AccessToken, default);
        var idToken = await _tokenStore.GetTokenProperty<string>(TokenPropertyNameEnum.IdToken, default);
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
        var claims = new List<Claim>(await _tokenClaimResolver.TransformAccessTokenClaims(accessToken, default));
        if (!string.IsNullOrWhiteSpace(idToken))
        {
            claims.AddRange(await _tokenClaimResolver.TransformIdTokenClaims(idToken, default));
        }
        var state = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme)));
        AuthenticationStateUser = state.User;
        return state;
    }

    private async Task RevalidationLoop(Task<AuthenticationState> authenticationStateTask, CancellationToken cancellationToken)
    {
        try
        {
            var authenticationState = await authenticationStateTask;
            if (authenticationState.User.Identity?.IsAuthenticated ?? false)
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    bool isValid;

                    try
                    {
                        await Task.Delay(_revalidationInterval, cancellationToken);
                        isValid = await ValidateAuthenticationStateAsync(authenticationState, cancellationToken);
                    }
                    catch (TaskCanceledException tce)
                    {
                        // If it was our cancellation token, then this revalidation loop gracefully completes
                        // Otherwise, treat it like any other failure
                        if (tce.CancellationToken == cancellationToken)
                        {
                            break;
                        }

                        throw;
                    }

                    if (!isValid)
                    {
                        await ForceSignOut(cancellationToken);
                        break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _messageDisplay.ShowError(ex.Message);
            await ForceSignOut(cancellationToken);
        }
    }

    private async Task ForceSignOut(CancellationToken cancellationToken)
    {
        await _identityService.Logout(LogoutTypeEnum.Local, cancellationToken);
        MarkUserAsLoggedOut();
    }

    /// <summary>
    /// Determines whether the authentication state is still valid.
    /// </summary>
    /// <param name="authenticationState">The current <see cref="AuthenticationState"/>.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while performing the operation.</param>
    /// <returns>A <see cref="Task"/> that resolves as true if the <paramref name="authenticationState"/> is still valid, or false if it is not.</returns>
    private async Task<bool> ValidateAuthenticationStateAsync(AuthenticationState authState, CancellationToken cancellationToken)
    {
        var user = authState.User;
        if (!user.Identity?.IsAuthenticated ?? true)
        {
            _messageDisplay.ShowError("User is not authenticated.");
            return false;
        }
        var accessTokenExpiresAtTimestampString = user.FindFirst(c => c.Type.Equals("exp"))?.Value;
        if (long.TryParse(accessTokenExpiresAtTimestampString, out var accessTokenExpiresAtTimestamp))
        {
            var accessTokenExpiresAt = DateTimeOffset.FromUnixTimeSeconds(accessTokenExpiresAtTimestamp);
            var utcNow = DateTime.UtcNow;
            //expires in less than 10 minutes or has already expired
            if (utcNow.AddMinutes(10) > accessTokenExpiresAt)
            {
                var ret = (await _identityService.RefreshToken(cancellationToken)).Succeeded;
                if (ret)
                    _messageDisplay.ShowSuccess("Refreshed sign in.");
                return ret;
            }
            //not expired, consider valid
            return true;
        }
        //Cannot determine access token expire time.
        _messageDisplay.ShowError("Cannot determine access token expire time.");
        return false;
    }


    void IDisposable.Dispose()
    {
        _loopCancellationTokenSource?.Cancel();
        Dispose(disposing: true);
    }

    /// <inheritdoc />
    protected virtual void Dispose(bool disposing)
    {
    }
}
