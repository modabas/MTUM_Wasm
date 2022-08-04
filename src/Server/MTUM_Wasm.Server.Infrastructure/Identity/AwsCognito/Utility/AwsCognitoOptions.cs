namespace MTUM_Wasm.Server.Infrastructure.Identity.AwsCognito.Utility;

internal class AwsCognitoOptions
{
    public string UserPoolId { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string AccessKeyId { get; set; } = string.Empty;
    public string SecretAccessKey { get; set; } = string.Empty;
    public string UserPoolClientId { get; set; } = string.Empty;
    public string UserPoolClientSecret { get; set; } = string.Empty;
    public int UserPoolClientRefreshTokenExpirationInMinutes { get; set; }
}
