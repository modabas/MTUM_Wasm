using MTUM_Wasm.Client.Core.Identity.Service;
using MTUM_Wasm.Client.Core.Utility.Preferences;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Threading.Tasks;

namespace MTUM_Wasm.Client.Web.Shared;

public partial class MainLayout
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Inject] IPreferencesService PreferencesService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private bool isDarkMode = false;
    private bool isDrawerOpen = true;
    private string modeIcon = Icons.Outlined.DarkMode;
    private ClientPreferences preferences = new();
    private MudTheme currentTheme = ApplicationTheme.DefaultTheme;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            preferences = await PreferencesService.GetPreferences();
            isDrawerOpen = preferences.IsDrawerOpen;
            isDarkMode = preferences.IsDarkMode;
            modeIcon = DetermineModeIcon();
            currentTheme = await PreferencesService.GetCurrentThemeAsync();
            StateHasChanged();
        }
    }

    private string DetermineModeIcon()
    {
        return isDarkMode ? Icons.Filled.LightMode : Icons.Outlined.DarkMode;
    }

    private async Task SwitchModeAsync()
    {
        isDarkMode = !isDarkMode;
        modeIcon = DetermineModeIcon();
        preferences.IsDarkMode = isDarkMode;
        await PreferencesService.SetPreferences(preferences);
        currentTheme = await PreferencesService.GetCurrentThemeAsync();
        StateHasChanged();
    }

    private async Task DrawerToggleAsync()
    {
        isDrawerOpen = !isDrawerOpen;
        preferences.IsDrawerOpen = isDrawerOpen;
        await PreferencesService.SetPreferences(preferences);
    }
}
