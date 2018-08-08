using Newtonsoft.Json;
using Switcheo.Net.Converters;

namespace Switcheo.Net.Objects
{
    /// <summary>
    /// Balance of a possessed asset
    /// </summary>
    public class SwitcheoAssetBalance
    {
        /// <summary>
        /// The asset
        /// </summary>
        public SupportedAsset Asset { get; set; }

        /// <summary>
        /// Amount of possessed asset
        /// </summary>
        [JsonConverter(typeof(NeoAssetAmountConverter))]
        public decimal Amount { get; set; }

        public override string ToString()
        {
            return string.Format("{{ Asset : {0}, Amount : {1} }}", this.Asset.ToString(), this.Amount);
        }
    }
}
