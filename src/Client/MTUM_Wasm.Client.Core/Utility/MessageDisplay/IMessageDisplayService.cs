using System.Collections.Generic;

namespace MTUM_Wasm.Client.Core.Utility.MessageDisplay;

internal interface IMessageDisplayService
{
    void ShowError(string message);
    void ShowError(IEnumerable<string> messages);
    void ShowSuccess(string message);
    void ShowSuccess(IEnumerable<string> messages);
    void ShowInformation(string message);
    void ShowInformation(IEnumerable<string> messages);
    void ShowWarning(string message);
    void ShowWarning(IEnumerable<string> messages);
}
