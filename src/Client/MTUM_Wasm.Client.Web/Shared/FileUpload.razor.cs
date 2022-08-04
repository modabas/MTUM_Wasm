using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTUM_Wasm.Client.Web.Shared;

public partial class FileUpload
{

    [Parameter] public EventCallback<InputFileChangeEventArgs> OnChange { get; set; }
    [Parameter] public string Label { get; set; } = string.Empty;
    [Parameter] public string TemplateFileLink { get; set; } = string.Empty;
    [Parameter] public bool AllowMultiple { get; set; } = false;

    private bool _clearing = false;
    private static readonly string _defaultDragClass = "relative rounded-lg border-2 border-dashed pa-4 mt-4 mud-width-full mud-height-full";
    private string _dragClass = _defaultDragClass;
    private readonly List<string> _fileNames = new();
    public readonly Dictionary<string, object> _inputAttributes = new();

    protected override Task OnInitializedAsync()
    {
        if (AllowMultiple)
            _inputAttributes["multiple"] = string.Empty;
        return Task.CompletedTask;
    }

    private async Task OnInputFileChanged(InputFileChangeEventArgs e)
    {
        ClearDragClass();
        var files = e.GetMultipleFiles();
        foreach (var file in files)
        {
            _fileNames.Add(file.Name);
        }
        if (OnChange.HasDelegate)
        {
            await OnChange.InvokeAsync(e);
        }
    }

    private async Task Clear()
    {
        _clearing = true;
        _fileNames.Clear();
        ClearDragClass();
        await Task.Delay(100);
        _clearing = false;
    }

    private void SetDragClass()
    {
        _dragClass = $"{_defaultDragClass} mud-border-primary";
    }

    private void ClearDragClass()
    {
        _dragClass = _defaultDragClass;
    }
}
