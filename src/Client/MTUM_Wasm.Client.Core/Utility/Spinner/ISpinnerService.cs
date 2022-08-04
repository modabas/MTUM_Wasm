using System;

namespace MTUM_Wasm.Client.Core.Utility.Spinner;

internal interface ISpinnerService
{
    event Action? OnHide;
    event Action? OnShow;

    void Hide();
    void Show();
}
