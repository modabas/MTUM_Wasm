using MTUM_Wasm.Client.Core.TenantAdmin.Service;
using MTUM_Wasm.Client.Core.Utility;
using MTUM_Wasm.Client.Core.Utility.MessageDisplay;
using MTUM_Wasm.Shared.Core.Common.Authorization;
using MTUM_Wasm.Shared.Core.Common.Dto;
using MTUM_Wasm.Shared.Core.TenantAdmin.Dto;
using MTUM_Wasm.Shared.Core.TenantAdmin.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTUM_Wasm.Client.Web.Pages.TenantAdmin;

[Route(PageUri.TenantAdmin.AuditLogs)]
[Authorize(Policy = Policy.Name.IsTenantAdmin)]
public partial class AuditLogs
{
    private SearchAuditLogsRequest _searchAuditLogsRequest = new();
    private readonly SearchAuditLogsRequestValidator _searchAuditLogsRequestValidator = new();
    private SearchAuditLogsRequest _validatedSearchAuditLogsRequest = new();
    private MudForm? _form;
    private MudDataGrid<AuditLogDto>? _grid;
    private bool _firstDataTableServerReload = true;

    private Dictionary<long, bool> _detailCollection = new();
    private static readonly GridData<AuditLogDto> EmptyTableData = new() { TotalItems = 0, Items = Enumerable.Empty<AuditLogDto>() };

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Inject] internal ITenantAdminService TenantAdminService { get; set; }
    [Inject] internal IMessageDisplayService MessageDisplayService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    protected override Task OnInitializedAsync()
    {
        _searchAuditLogsRequest.StartDate = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Unspecified);
        _searchAuditLogsRequest.EndDate = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Unspecified);
        return Task.CompletedTask;
    }

    private async Task<GridData<AuditLogDto>> ServerReload(GridState<AuditLogDto> state)
    {
        try
        {
            //will be fired on page load, skip it
            if (_firstDataTableServerReload)
            {
                _firstDataTableServerReload = false;
                _detailCollection.Clear();
                return EmptyTableData;
            }


            _validatedSearchAuditLogsRequest.Page = state.Page + 1;
            _validatedSearchAuditLogsRequest.PageSize = state.PageSize;

            var results = await TenantAdminService.SearchAuditLogs(_validatedSearchAuditLogsRequest, default);
            if (results.Succeeded && results.Data is not null)
            {
                _detailCollection = results.Data.Logs.Select(i=>new { Id = i.Id, ShowDetails = false }).ToDictionary(p=>p.Id, p=>p.ShowDetails);
                return new GridData<AuditLogDto>() { TotalItems = (int)results.Data.TotalCount, Items = results.Data.Logs };
            }
            if (!results.Succeeded)
            {
                MessageDisplayService.ShowError(results.Messages);
            }
            else
            {
                MessageDisplayService.ShowError("Cannot fetch audit logs.");
            }
            _detailCollection.Clear();
            return EmptyTableData;
        }
        catch (Exception ex)
        {
            MessageDisplayService.ShowError(ex.Message);
            _detailCollection.Clear();
            return EmptyTableData;
        }
    }

    private async Task Search()
    {
        try
        {
            if (_form is null || _grid is null)
            {
                MessageDisplayService.ShowError("Critical error. Try restarting application.");
                return;
            }
            await _form.Validate();
            if (_form.IsValid)
            {
                _validatedSearchAuditLogsRequest.StartDate = _searchAuditLogsRequest.StartDate;
                _validatedSearchAuditLogsRequest.EndDate = _searchAuditLogsRequest.EndDate;
                _validatedSearchAuditLogsRequest.UserEmail = _searchAuditLogsRequest.UserEmail;
                _validatedSearchAuditLogsRequest.CommandName = _searchAuditLogsRequest.CommandName;
                await _grid.ReloadServerData();
            }
        }
        catch (Exception ex)
        {
            MessageDisplayService.ShowError(ex.Message);
        }
    }

    private Task OnAuditTableRowClick(DataGridRowClickEventArgs<AuditLogDto> row)
    {
        try
        {
            //Show/hide row details
            if (row.Item is not null)
            {
                var id = row.Item.Id;
                var currentDetailStatus = _detailCollection.GetValueOrDefault(id);
                _detailCollection[id] = !currentDetailStatus;
            }
        }
        catch (Exception ex)
        {
            MessageDisplayService.ShowError(ex.Message);
        }
        return Task.CompletedTask;
    }



}
