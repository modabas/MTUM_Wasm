using MTUM_Wasm.Client.Core.Identity.Http;
using MTUM_Wasm.Client.Core.Utility;
using MTUM_Wasm.Shared.Core.Common.Result;
using MTUM_Wasm.Shared.Core.Common.Utility;
using MTUM_Wasm.Shared.Core.Identity.Dto;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Client.Core.Identity.Service;

internal class IdentityService : IIdentityService
{
    private readonly HttpClient _httpClient;
    private readonly ITokenStore _tokenStore;

    private const string LoginUri = "api/Identity/login";
    private const string RefreshUri = "api/Identity/refresh";
    private const string LogoutUri = "api/Identity/logout";
    private const string ChangePasswordUri = "api/Identity/changePassword";
    private const string ForgotPasswordUri = "api/Identity/forgotPassword";
    private const string ResetPasswordUri = "api/Identity/resetPassword";

    public IdentityService(HttpClient httpClient, ITokenStore tokenStore)
    {
        _httpClient = httpClient;
        _tokenStore = tokenStore;
    }

    public async Task<IServiceResult> Login(LoginRequest loginRequest, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync(LoginUri, loginRequest, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.DeserializeResult<LoginResponse>(cancellationToken);
        if (result.Succeeded)
        {
            var accessToken = result.Data?.AccessToken ?? string.Empty;
            var idToken = result.Data?.IdToken ?? string.Empty;
            var refreshToken = result.Data?.RefreshToken ?? string.Empty;
            var refreshTokenExpiresAt = result.Data?.RefreshTokenExpiresAt ?? DateTime.MinValue.ToUniversalTime();
            await _tokenStore.SetTokenProperty(TokenPropertyNameEnum.AccessToken, accessToken, cancellationToken);
            await _tokenStore.SetTokenProperty(TokenPropertyNameEnum.IdToken, idToken, cancellationToken);
            await _tokenStore.SetTokenProperty(TokenPropertyNameEnum.RefreshToken, refreshToken, cancellationToken);
            await _tokenStore.SetTokenProperty(TokenPropertyNameEnum.RefreshTokenExpiresAt, refreshTokenExpiresAt, cancellationToken);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Definitions.BearerSchemeName, accessToken);
            _httpClient.DefaultRequestHeaders.Set(HttpHeader.Name.IdToken, idToken);

            return Result.Success();
        }
        else
        {
            return Result.Fail(result.Messages);
        }
    }

    public async Task<IServiceResult<RefreshResponse>> RefreshToken(CancellationToken cancellationToken)
    {
        var refreshToken = await _tokenStore.GetTokenProperty<string>(TokenPropertyNameEnum.RefreshToken, cancellationToken);
        var refreshTokenExpiresAt = await _tokenStore.GetTokenProperty<DateTime>(TokenPropertyNameEnum.RefreshTokenExpiresAt, cancellationToken);
        var response = await _httpClient.PostAsJsonAsync(RefreshUri, new RefreshRequest() { RefreshToken = refreshToken, RefreshTokenExpiresAt = refreshTokenExpiresAt }, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.DeserializeResult<RefreshResponse>(cancellationToken);

        if (result.Succeeded)
        {
            var accessToken = result.Data?.AccessToken ?? string.Empty;
            var idToken = result.Data?.IdToken ?? string.Empty;
            refreshToken = result.Data?.RefreshToken ?? string.Empty;
            refreshTokenExpiresAt = result.Data?.RefreshTokenExpiresAt ?? DateTime.MinValue.ToUniversalTime();
            await _tokenStore.SetTokenProperty(TokenPropertyNameEnum.AccessToken, accessToken, cancellationToken);
            await _tokenStore.SetTokenProperty(TokenPropertyNameEnum.IdToken, idToken, cancellationToken);
            await _tokenStore.SetTokenProperty(TokenPropertyNameEnum.RefreshToken, refreshToken, cancellationToken);
            await _tokenStore.SetTokenProperty(TokenPropertyNameEnum.RefreshTokenExpiresAt, refreshTokenExpiresAt, cancellationToken);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Definitions.BearerSchemeName, accessToken);
            _httpClient.DefaultRequestHeaders.Set(HttpHeader.Name.IdToken, idToken);

            return result;
        }
        else
        {
            return Result<RefreshResponse>.Fail(result.Messages);
        }
    }

    public async Task<IServiceResult> Logout(LogoutTypeEnum logoutType, CancellationToken cancellationToken)
    {
        switch (logoutType)
        {
            case LogoutTypeEnum.Global:
                var response = await _httpClient.GetAsync(LogoutUri, cancellationToken);
                response.EnsureSuccessStatusCode();
                var result = await response.DeserializeResult(cancellationToken);
                if (result.Succeeded)
                    await PerformLocalLogout(cancellationToken);
                return result;

            case LogoutTypeEnum.Local:
                await PerformLocalLogout(cancellationToken);
                return Result.Success();

            default:
                return Result.Fail($"Undefined logout type: {Enum.GetName<LogoutTypeEnum>(logoutType)}");
        }
    }

    private async Task PerformLocalLogout(CancellationToken cancellationToken)
    {
        await _tokenStore.RemoveTokenProperty(TokenPropertyNameEnum.AccessToken, cancellationToken);
        await _tokenStore.RemoveTokenProperty(TokenPropertyNameEnum.IdToken, cancellationToken);
        await _tokenStore.RemoveTokenProperty(TokenPropertyNameEnum.RefreshToken, cancellationToken);
        await _tokenStore.RemoveTokenProperty(TokenPropertyNameEnum.RefreshTokenExpiresAt, cancellationToken);
        _httpClient.DefaultRequestHeaders.Authorization = null;
        _httpClient.DefaultRequestHeaders.Remove(HttpHeader.Name.IdToken);
    }

    public async Task<IServiceResult> ChangePassword(ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync(ChangePasswordUri, request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.DeserializeResult(cancellationToken);
        return result;
    }

    public async Task<IServiceResult> ForgotPassword(ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync(ForgotPasswordUri, request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.DeserializeResult(cancellationToken);
        return result;
    }

    public async Task<IServiceResult> ResetPassword(ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync(ResetPasswordUri, request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.DeserializeResult(cancellationToken);
        return result;
    }
}
