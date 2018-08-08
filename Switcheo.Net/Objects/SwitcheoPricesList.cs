using Newtonsoft.Json;
using Switcheo.Net.Converters;

namespace Switcheo.Net.Objects
{
    /// <summary>
    /// Result of asking price(s) of symbol(s)
    /// </summary>
    [JsonConverter(typeof(PricesListConverter))]
    public class SwitcheoPricesList
    {
        /// <summary>
        /// The prices
        /// </summary>
        [JsonIgnore]
        public SwitcheoPrice[] Prices { get; set; }
    }
}
