using Blazored.LocalStorage;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Client.Core.Identity.Service;

internal class LocalStorageTokenStore : ITokenStore
{
    private readonly ILocalStorageService _localStorage;

    private const string LocalStorageKeyForAccessToken = "accessToken";
    private const string LocalStorageKeyForIdToken = "idToken";
    private const string LocalStorageKeyForRefreshToken = "refreshToken";
    private const string LocalStorageKeyForRefreshTokenExpiresAt = "refreshTokenExpiresAt";


    public LocalStorageTokenStore(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task<T> GetTokenProperty<T>(TokenPropertyNameEnum tokenPropertyName, CancellationToken cancellationToken)
    {
        return tokenPropertyName switch
        {
            TokenPropertyNameEnum.AccessToken => await _localStorage.GetItemAsync<T>(LocalStorageKeyForAccessToken, cancellationToken),
            TokenPropertyNameEnum.IdToken => await _localStorage.GetItemAsync<T>(LocalStorageKeyForIdToken, cancellationToken),
            TokenPropertyNameEnum.RefreshToken => await _localStorage.GetItemAsync<T>(LocalStorageKeyForRefreshToken, cancellationToken),
            TokenPropertyNameEnum.RefreshTokenExpiresAt => await _localStorage.GetItemAsync<T>(LocalStorageKeyForRefreshTokenExpiresAt, cancellationToken),
            _ => throw new ArgumentOutOfRangeException(nameof(tokenPropertyName), tokenPropertyName, "Unexpected token property name."),
        };
    }

    public async Task SetTokenProperty<T>(TokenPropertyNameEnum tokenPropertyName, T value, CancellationToken cancellationToken)
    {
        switch (tokenPropertyName)
        {
            case TokenPropertyNameEnum.AccessToken:
                await _localStorage.SetItemAsync(LocalStorageKeyForAccessToken, value, cancellationToken);
                break;

            case TokenPropertyNameEnum.IdToken:
                await _localStorage.SetItemAsync(LocalStorageKeyForIdToken, value, cancellationToken);
                break;

            case TokenPropertyNameEnum.RefreshToken:
                await _localStorage.SetItemAsync(LocalStorageKeyForRefreshToken, value, cancellationToken);
                break;

            case TokenPropertyNameEnum.RefreshTokenExpiresAt:
                await _localStorage.SetItemAsync(LocalStorageKeyForRefreshTokenExpiresAt, value, cancellationToken);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(tokenPropertyName), tokenPropertyName, "Unexpected token property name.");
        }
    }

    public async Task RemoveTokenProperty(TokenPropertyNameEnum tokenPropertyName, CancellationToken cancellationToken)
    {
        switch (tokenPropertyName)
        {
            case TokenPropertyNameEnum.AccessToken:
                await _localStorage.RemoveItemAsync(LocalStorageKeyForAccessToken, cancellationToken);
                break;

            case TokenPropertyNameEnum.IdToken:
                await _localStorage.RemoveItemAsync(LocalStorageKeyForIdToken, cancellationToken);
                break;

            case TokenPropertyNameEnum.RefreshToken:
                await _localStorage.RemoveItemAsync(LocalStorageKeyForRefreshToken, cancellationToken);
                break;

            case TokenPropertyNameEnum.RefreshTokenExpiresAt:
                await _localStorage.RemoveItemAsync(LocalStorageKeyForRefreshTokenExpiresAt, cancellationToken);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(tokenPropertyName), tokenPropertyName, "Unexpected token property name.");
        }
    }
}
