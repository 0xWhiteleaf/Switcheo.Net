using CryptoExchange.Net.Converters;
using Newtonsoft.Json;
using System;

namespace Switcheo.Net.Objects
{
    /// <summary>
    /// Information about Switcheo API's server time
    /// </summary>
    public class SwitcheoCheckTime
    {
        [JsonProperty("timestamp"), JsonConverter(typeof(TimestampConverter))]
        public DateTime ServerTime { get; set; }
    }
}
