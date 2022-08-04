using MTUM_Wasm.Server.Core.Common.Utility;
using Microsoft.Extensions.Logging;
using Orleans;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Core.Common.Orleans;

/// <summary>
/// This grain demonstrates a simple way to throttle a given client (identified by their IP address, which is used as the primary key of the grain)
/// It uses a call filter to maintain a count of recent calls and throttles if they exceed a defined threshold. The score decays over time until, allowing
/// the client to resume making calls.
/// </summary>
internal class ThrottlingGrain : Grain, IThrottlingGrain, IIncomingGrainCallFilter
{
    private const int ThrottleThreshold = 3;
    private const int DecayPeriod = 5;
    private const double DecayRate = (double)ThrottleThreshold / (double)DecayPeriod;
    private double _throttleScore;
    private Stopwatch _stopwatch = new Stopwatch();

    public async Task Invoke(IIncomingGrainCallContext context)
    {
        // Work out how long it's been since the last call
        var elapsedSeconds = _stopwatch.Elapsed.TotalSeconds;
        _stopwatch.Restart();

        // Calculate a new score based on a constant rate of score decay and the time which elapsed since the last call.
        _throttleScore = Math.Max(0, _throttleScore - elapsedSeconds * DecayRate) + 1;

        // If the user has exceeded the threshold, deny their request and give them a helpful warning.
        if (_throttleScore > ThrottleThreshold)
        {
            var remainingSeconds = Math.Max(0, (int)Math.Ceiling((_throttleScore - (ThrottleThreshold - 1)) / DecayRate));
            throw new ThrottlingException($"Request rate exceeded, wait {remainingSeconds}s before retrying.");
        }

        await context.Invoke();
    }

    public ValueTask Poke()
    {
        return ValueTask.CompletedTask;
    }
}
