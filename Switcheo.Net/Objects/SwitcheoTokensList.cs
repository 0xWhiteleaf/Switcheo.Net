using Newtonsoft.Json;
using Switcheo.Net.Converters;

namespace Switcheo.Net.Objects
{
    /// <summary>
    /// Result of asking tokens list
    /// </summary>
    [JsonConverter(typeof(TokensListConverter))]
    public class SwitcheoTokensList
    {
        /// <summary>
        /// Tokens list
        /// </summary>
        public SwitcheoToken[] Tokens { get; set; }
    }
}
