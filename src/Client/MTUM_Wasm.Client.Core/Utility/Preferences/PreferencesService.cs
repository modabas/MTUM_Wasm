using Blazored.LocalStorage;
using MudBlazor;
using System.Threading.Tasks;

namespace MTUM_Wasm.Client.Core.Utility.Preferences;

internal class PreferencesService : IPreferencesService
{
    private readonly ILocalStorageService _localStorageService;

    public PreferencesService(
        ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
    }

    public async Task<bool> ToggleDarkModeAsync()
    {
        var preference = await GetPreferences() as ClientPreferences;
        if (preference is not null)
        {
            preference.IsDarkMode = !preference.IsDarkMode;
            await SetPreferences(preference);
            return !preference.IsDarkMode;
        }

        return false;
    }

    public async Task<MudTheme> GetCurrentThemeAsync()
    {
        var preference = await GetPreferences() as ClientPreferences;
        if (preference is not null)
        {
            if (preference.IsDarkMode == true) return ApplicationTheme.DarkTheme;
        }
        return ApplicationTheme.DefaultTheme;
    }

    public async Task<ClientPreferences> GetPreferences()
    {
        return await _localStorageService.GetItemAsync<ClientPreferences>(Definitions.LocalStorageKeyForClientPreferences) ?? new ClientPreferences();
    }

    public async Task SetPreferences(ClientPreferences preferences)
    {
        await _localStorageService.SetItemAsync(Definitions.LocalStorageKeyForClientPreferences, preferences as ClientPreferences);
    }
}
