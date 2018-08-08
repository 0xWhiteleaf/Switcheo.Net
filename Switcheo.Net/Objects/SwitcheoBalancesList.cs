using Newtonsoft.Json;
using Switcheo.Net.Converters;

namespace Switcheo.Net.Objects
{
    /// <summary>
    /// Result of asking balances of a contract
    /// </summary>
    [JsonConverter(typeof(BalancesListConverter))]
    public class SwitcheoBalancesList
    {
        /// <summary>
        /// Confirming balances
        /// </summary>
        [JsonIgnore]
        public SwitcheoAssetConfirming[] Confirming { get; set; }

        /// <summary>
        /// Confirmed balances
        /// </summary>
        [JsonIgnore]
        public SwitcheoAssetBalance[] Confirmed { get; set; }

        /// <summary>
        /// Locked balances
        /// </summary>
        [JsonIgnore]
        public SwitcheoAssetBalance[] Locked { get; set; }
    }
}
