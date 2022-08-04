using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MTUM_Wasm.Client.Web.Shared;

public partial class SpinnerFoldingCube
{
    [Parameter]
    public MudColor Color { get; set; } = new MudColor("#333");
}
