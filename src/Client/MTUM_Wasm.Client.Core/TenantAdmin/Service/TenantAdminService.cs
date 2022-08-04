using MTUM_Wasm.Client.Core.Utility.QueryHelpers;
using MTUM_Wasm.Shared.Core.Common.Result;
using MTUM_Wasm.Shared.Core.TenantAdmin.Dto;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Client.Core.TenantAdmin.Service;

internal class TenantAdminService : ITenantAdminService
{
    private readonly HttpClient _httpClient;

    private const string GetUsersUri = "/api/TenantAdministration/getUsers";
    private const string UpdateUserAttributesUri = "/api/TenantAdministration/updateUserAttributes";
    private const string CreateUserUri = "/api/TenantAdministration/createUser";
    private const string ChangeUserStateUri = "/api/TenantAdministration/changeUserState";
    private const string GetUserGroupsUri = "/api/TenantAdministration/getUserGroups";
    private const string GetUserUri = "/api/TenantAdministration/getUser";
    private const string UpdateUserGroupsUri = "/api/TenantAdministration/updateUserGroups";
    private const string SearchAuditLogsUri = "/api/TenantAdministration/searchAuditLogs";
    private const string UpdateUserNacPolicyUri = "/api/TenantAdministration/updateUserNacPolicy";
    private const string UpdateTenantNacPolicyUri = "/api/TenantAdministration/updateTenantNacPolicy";

    public TenantAdminService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IServiceResult<GetUsersResponse>> GetUsers(CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(GetUsersUri, cancellationToken: cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.DeserializeResult<GetUsersResponse>(cancellationToken);

        return result;
    }

    public async Task<IServiceResult<GetUserGroupsResponse>> GetUserGroups(GetUserGroupsRequest request, CancellationToken cancellationToken)
    {
        var uri = QueryHelpers.AddQueryString(GetUserGroupsUri, "Email", request.Email);
        var response = await _httpClient.GetAsync(uri, cancellationToken: cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.DeserializeResult<GetUserGroupsResponse>(cancellationToken);

        return result;
    }

    public async Task<IServiceResult<GetUserResponse>> GetUser(GetUserRequest request, CancellationToken cancellationToken)
    {
        var uri = QueryHelpers.AddQueryString(GetUserUri, "Email", request.Email);
        var response = await _httpClient.GetAsync(uri, cancellationToken: cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.DeserializeResult<GetUserResponse>(cancellationToken);

        return result;
    }

    public async Task<IServiceResult> UpdateUserAttributes(UpdateUserAttributesRequest request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync(UpdateUserAttributesUri, request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.DeserializeResult(cancellationToken);
        return result;
    }

    public async Task<IServiceResult> CreateUser(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync(CreateUserUri, request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.DeserializeResult(cancellationToken);
        return result;
    }

    public async Task<IServiceResult> ChangeUserState(ChangeUserStateRequest request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync(ChangeUserStateUri, request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.DeserializeResult(cancellationToken);
        return result;
    }

    public async Task<IServiceResult> UpdateUserGroups(UpdateUserGroupsRequest request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync(UpdateUserGroupsUri, request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.DeserializeResult(cancellationToken);
        return result;
    }

    public async Task<IServiceResult<SearchAuditLogsResponse>> SearchAuditLogs(SearchAuditLogsRequest request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync(SearchAuditLogsUri, request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.DeserializeResult<SearchAuditLogsResponse>(cancellationToken);
        return result;
    }

    public async Task<IServiceResult> UpdateUserNacPolicy(UpdateUserNacPolicyRequest request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync(UpdateUserNacPolicyUri, request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.DeserializeResult(cancellationToken);
        return result;
    }

    public async Task<IServiceResult> UpdateTenantNacPolicy(UpdateTenantNacPolicyRequest request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync(UpdateTenantNacPolicyUri, request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.DeserializeResult(cancellationToken);
        return result;
    }
}
