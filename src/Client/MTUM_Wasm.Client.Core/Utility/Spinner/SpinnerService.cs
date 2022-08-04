using System;

namespace MTUM_Wasm.Client.Core.Utility.Spinner;

internal class SpinnerService : ISpinnerService
{
    public event Action? OnShow;
    public event Action? OnHide;

    public void Show()
    {
        OnShow?.Invoke();
    }

    public void Hide()
    {
        OnHide?.Invoke();
    }
}
