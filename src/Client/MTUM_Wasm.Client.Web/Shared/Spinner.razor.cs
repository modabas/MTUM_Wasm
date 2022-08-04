using MTUM_Wasm.Client.Core.Utility.Preferences;
using MTUM_Wasm.Client.Core.Utility.Spinner;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MTUM_Wasm.Client.Web.Shared;

public partial class Spinner
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Inject] internal ISpinnerService SpinnerService { get; set; }
    [Inject] IPreferencesService PreferencesService { get; set; }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    protected MudTheme currentTheme = ApplicationTheme.DefaultTheme;
    protected bool newVisibleState { get; set; }
    protected bool oldVisibleState { get; set; }
    protected object lockObject = new object();

    protected override async void OnInitialized()
    {
        SpinnerService.OnShow += ShowSpinner;
        SpinnerService.OnHide += HideSpinner;
        currentTheme = await PreferencesService.GetCurrentThemeAsync();
    }

    public void ShowSpinner()
    {
        bool visibleStateChanged;
        lock (lockObject)
        {
            oldVisibleState = newVisibleState;
            newVisibleState = true;
            visibleStateChanged = newVisibleState != oldVisibleState;
        }
        if (visibleStateChanged)
            StateHasChanged();
    }

    public void HideSpinner()
    {
        bool visibleStateChanged;
        lock (lockObject)
        {
            oldVisibleState = newVisibleState;
            newVisibleState = false;
            visibleStateChanged = newVisibleState != oldVisibleState;
        }
        if (visibleStateChanged)
            StateHasChanged();
    }
}
