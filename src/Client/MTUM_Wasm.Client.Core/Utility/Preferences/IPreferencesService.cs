using MudBlazor;
using System.Threading.Tasks;

namespace MTUM_Wasm.Client.Core.Utility.Preferences
{
    internal interface IPreferencesService
    {
        Task<MudTheme> GetCurrentThemeAsync();
        Task<ClientPreferences> GetPreferences();
        Task SetPreferences(ClientPreferences preferences);
        Task<bool> ToggleDarkModeAsync();
    }
}