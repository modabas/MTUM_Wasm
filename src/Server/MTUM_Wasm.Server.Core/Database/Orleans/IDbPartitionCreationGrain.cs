using Orleans;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Core.Database.Orleans;

internal interface IDbPartitionCreationGrain : IGrainWithIntegerKey
{
    Task<bool> IsBusy();
    Task Poke();
}
