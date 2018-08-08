using System;

namespace Switcheo.Net.Helpers
{
    public static class DatesHelper
    {
        public static DateTime FromTimestamp(long timestamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timestamp);
        }
    }
}
