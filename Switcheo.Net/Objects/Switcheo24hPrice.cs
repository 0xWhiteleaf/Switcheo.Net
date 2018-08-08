using Newtonsoft.Json;

namespace Switcheo.Net.Objects
{
    /// <summary>
    /// Price statistics of the last 24 hours
    /// </summary>
    public class Switcheo24hPrice
    {
        /// <summary>
        /// The pair the price is for
        /// </summary>
        [JsonProperty("pair")]
        public string Pair { get; set; }

        /// <summary>
        /// The open price 24 hours ago
        /// </summary>
        [JsonProperty("open")]
        public decimal Open { get; set; }

        /// <summary>
        /// The close price
        /// </summary>
        [JsonProperty("close")]
        public decimal Close { get; set; }

        /// <summary>
        /// The highest price in the last 24 hours
        /// </summary>
        [JsonProperty("high")]
        public decimal High { get; set; }

        /// <summary>
        /// The lowest price in the last 24 hours
        /// </summary>
        [JsonProperty("low")]
        public decimal Low { get; set; }

        /// <summary>
        /// The volume traded in the last 24 hours
        /// </summary>
        [JsonProperty("volume")]
        public decimal Volume { get; set; }

        /// <summary>
        /// The quote asset volume traded in the last 24 hours
        /// </summary>
        [JsonProperty("quote_volume")]
        public decimal QuoteVolume { get; set; }

        public override string ToString()
        {
            return string.Format("{{ Pair : {0}, Open : {1}, Close : {2}, High : {3}, Low : {4}, Volume : {5}, QuoteVolume : {6} }}", this.Pair,
                this.Open, this.Close, this.High, this.Low, this.Volume, this.QuoteVolume);
        }
    }
}
