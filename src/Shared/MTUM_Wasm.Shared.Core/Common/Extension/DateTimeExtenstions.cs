using System;

namespace MTUM_Wasm.Shared.Core.Common.Extension
{
    public static class DateTimeExtenstions
    {
        public static DateTime FromUnixTimestamp(this DateTime dateTime, long timestamp)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddTicks(timestamp * TimeSpan.TicksPerSecond);
        }
    }
}
