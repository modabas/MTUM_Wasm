using Orleans;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Core.Common.Orleans;

internal interface IThrottlingGrain : IGrainWithStringKey
{
    ValueTask Poke();
}
