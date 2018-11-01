using Newtonsoft.Json;
using Switcheo.Net.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Switcheo.Net.Objects
{
    /// <summary>
    /// Information about an RPC node
    /// </summary>
    public class SwitcheoNode
    {
        /// <summary>
        /// The type of network
        /// </summary>
        [JsonProperty("net")]
        [JsonConverter(typeof(NetTypeConverter))]
        public NetType NetworkType { get; set; }

        /// <summary>
        /// The address (http(s)://sub.domain.com:port)
        /// </summary>
        [JsonProperty("node")]
        public string Address { get; set; }
    }
}
