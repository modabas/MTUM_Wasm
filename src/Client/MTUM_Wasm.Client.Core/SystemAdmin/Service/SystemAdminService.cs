using MTUM_Wasm.Client.Core.Utility.QueryHelpers;
using MTUM_Wasm.Shared.Core.Common.Result;
using MTUM_Wasm.Shared.Core.SystemAdmin.Dto;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Client.Core.SystemAdmin.Service;

internal class SystemAdminService : ISystemAdminService
{
    private readonly HttpClient _httpClient;

    private const string GetTenantsUri = "/api/SystemAdministration/getTenants";
    private const string CreateTenantUri = "/api/SystemAdministration/createTenant";
    private const string GetTenantUri = "/api/SystemAdministration/getTenant";
    private const string UpdateTenantUri = "/api/SystemAdministration/updateTenant";
    private const string GetTenantUsersUri = "/api/SystemAdministration/getTenantUsers";
    private const string CreateUserUri = "/api/SystemAdministration/createUser";
    private const string ChangeUserStateUri = "/api/SystemAdministration/changeUserState";
    private const string UpdateUserAttributesUri = "/api/SystemAdministration/updateUserAttributes";
    private const string GetUserUri = "/api/SystemAdministration/getUser";
    private const string GetUserGroupsUri = "/api/SystemAdministration/getUserGroups";
    private const string UpdateUserGroupsUri = "/api/SystemAdministration/updateUserGroups";
    private const string SearchAuditLogsUri = "/api/SystemAdministration/searchAuditLogs";
    private const string GetUsersInGroupUri = "/api/SystemAdministration/getUsersInGroup";
    private const string UpdateUserNacPolicyUri = "/api/SystemAdministration/updateUserNacPolicy";
    private const string UpdateTenantNacPolicyUri = "/api/SystemAdministration/updateTenantNacPolicy";

    public SystemAdminService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IServiceResult<GetTenantsResponse>> GetTenants(CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(GetTenantsUri, cancellationToken: cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.DeserializeResult<GetTenantsResponse>(cancellationToken);

        return result;
    }

    public async Task<IServiceResult> CreateTenant(CreateTenantRequest request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync(CreateTenantUri, request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.DeserializeResult(cancellationToken);
        return result;
    }

    public async Task<IServiceResult<GetTenantResponse>> GetTenant(GetTenantRequest request, CancellationToken cancellationToken)
    {
        var uri = QueryHelpers.AddQueryString(GetTenantUri, "Id", request.Id.ToString());
        var response = await _httpClient.GetAsync(uri, cancellationToken: cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.DeserializeResult<GetTenantResponse>(cancellationToken);

        return result;
    }

    public async Task<IServiceResult> UpdateTenant(UpdateTenantRequest request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync(UpdateTenantUri, request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.DeserializeResult(cancellationToken);
        return result;
    }

    public async Task<IServiceResult<GetTenantUsersResponse>> GetTenantUsers(GetTenantUsersRequest request, CancellationToken cancellationToken)
    {
        var uri = QueryHelpers.AddQueryString(GetTenantUsersUri, "TenantId", request.TenantId.ToString());
        var response = await _httpClient.GetAsync(uri, cancellationToken: cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.DeserializeResult<GetTenantUsersResponse>(cancellationToken);

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

    public async Task<IServiceResult> UpdateUserAttributes(UpdateUserAttributesRequest request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync(UpdateUserAttributesUri, request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.DeserializeResult(cancellationToken);
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

    public async Task<IServiceResult<GetUserGroupsResponse>> GetUserGroups(GetUserGroupsRequest request, CancellationToken cancellationToken)
    {
        var uri = QueryHelpers.AddQueryString(GetUserGroupsUri, "Email", request.Email);
        var response = await _httpClient.GetAsync(uri, cancellationToken: cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.DeserializeResult<GetUserGroupsResponse>(cancellationToken);

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

    public async Task<IServiceResult<GetUsersInGroupResponse>> GetUsersInGroup(GetUsersInGroupRequest request, CancellationToken cancellationToken)
    {
        var uri = QueryHelpers.AddQueryString(GetUsersInGroupUri, "GroupName", request.GroupName);
        var response = await _httpClient.GetAsync(uri, cancellationToken: cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.DeserializeResult<GetUsersInGroupResponse>(cancellationToken);

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
