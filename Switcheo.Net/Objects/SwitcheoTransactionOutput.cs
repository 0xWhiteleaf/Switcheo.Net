using Newtonsoft.Json;
using Switcheo.Net.Converters;

namespace Switcheo.Net.Objects
{
    public class SwitcheoTransactionOutput
    {
        /// <summary>
        /// The asset
        /// </summary>
        [JsonProperty("assetId")]
        [JsonConverter(typeof(SupportedAssetConverter), true)]
        public SupportedAsset Asset { get; set; }

        /// <summary>
        /// The script hash
        /// </summary>
        [JsonProperty("scriptHash")]
        public string ScriptHash { get; set; }

        /// <summary>
        /// The value
        /// </summary>
        [JsonProperty("value")]
        public decimal Value { get; set; }

        public override string ToString()
        {
            return string.Format("{{ Asset : {0}, ScriptHash : {1}, Value : {2} }}", this.Asset.ToString(), this.ScriptHash, this.Value);
        }
    }
}
