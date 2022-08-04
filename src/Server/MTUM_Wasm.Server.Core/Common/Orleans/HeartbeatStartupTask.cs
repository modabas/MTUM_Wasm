using MTUM_Wasm.Server.Core.Database.Orleans;
using Orleans;
using Orleans.Runtime;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Core.Common.Orleans;

internal class HeartbeatStartupTask : IStartupTask
{
    private readonly IGrainFactory _grainFactory;

    public HeartbeatStartupTask(IGrainFactory grainFactory)
    {
        this._grainFactory = grainFactory;
    }

    public async Task Execute(CancellationToken cancellationToken)
    {
        var grain = this._grainFactory.GetGrain<IDbPartitionCreationGrain>(0);
        await grain.Poke();
    }
}