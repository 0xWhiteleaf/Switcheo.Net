using Newtonsoft.Json;
using Switcheo.Net.Converters;

namespace Switcheo.Net.Objects
{
    /// <summary>
    /// List of all differents contracts used by Switcheo
    /// For more informations about contracts visit <a href="https://docs.switcheo.network/#contracts"/>#contracts</a>
    /// You can also take a look at <a href="https://docs.switcheo.network/#address"/>#address</a>
    /// </summary>
    [JsonConverter(typeof(ContractsListConverter))]
    public class SwitcheoContractsList
    {
        /// <summary>
        /// The used Neo contracts
        /// </summary>
        [JsonIgnore]
        public SwitcheoContract[] NeoContracts { get; set; }

        /// <summary>
        /// The used Qtum contracts
        /// </summary>
        [JsonIgnore]
        public SwitcheoContract[] QtumContracts { get; set; }

        /// <summary>
        /// The used Ethereum contracts
        /// </summary>
        [JsonIgnore]
        public SwitcheoContract[] EthereumContracts { get; set; }
    }
}
