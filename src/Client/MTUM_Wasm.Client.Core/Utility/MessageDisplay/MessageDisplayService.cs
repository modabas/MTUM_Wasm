using MudBlazor;
using System.Collections.Generic;

namespace MTUM_Wasm.Client.Core.Utility.MessageDisplay;

internal class MessageDisplayService : IMessageDisplayService
{
    private readonly ISnackbar _snackbar;
    public MessageDisplayService(ISnackbar snackbar)
    {
        _snackbar = snackbar;
    }
    public void ShowSuccess(string message)
    {
        _snackbar.Add(message, Severity.Success);
    }
    public void ShowSuccess(IEnumerable<string> messages)
    {
        ShowSuccess(string.Join("<br>", messages));
    }
    public void ShowError(string message)
    {
        _snackbar.Add(message, Severity.Error);
    }
    public void ShowError(IEnumerable<string> messages)
    {
        ShowError(string.Join("<br>", messages));
    }
    public void ShowInformation(string message)
    {
        _snackbar.Add(message, Severity.Info);
    }
    public void ShowInformation(IEnumerable<string> messages)
    {
        ShowInformation(string.Join("<br>", messages));
    }
    public void ShowWarning(string message)
    {
        _snackbar.Add(message, Severity.Warning);
    }
    public void ShowWarning(IEnumerable<string> messages)
    {
        ShowWarning(string.Join("<br>", messages));
    }
}
