using CryptoExchange.Net.Converters;
using Newtonsoft.Json;
using System;

namespace Switcheo.Net.Objects
{
    /// <summary>
    /// Candlestick information for pair
    /// </summary>
    public class SwitcheoCandlestick
    {
        /// <summary>
        /// The time this candlestick opened
        /// </summary>
        [JsonProperty("time")]
        [JsonConverter(typeof(TimestampSecondsConverter))]
        public DateTime Time { get; set; }

        /// <summary>
        /// The price at which this candlestick opened
        /// </summary>
        [JsonProperty("open")]
        public decimal Open { get; set; }

        /// <summary>
        /// The price at which this candlestick closed
        /// </summary>
        [JsonProperty("close")]
        public decimal Close { get; set; }

        /// <summary>
        /// The highest price in this candlestick
        /// </summary>
        [JsonProperty("high")]
        public decimal High { get; set; }

        /// <summary>
        /// The lowest price in this candlestick
        /// </summary>
        [JsonProperty("low")]
        public decimal Low { get; set; }

        /// <summary>
        /// The volume traded during this candlestick
        /// </summary>
        [JsonProperty("volume")]
        public decimal Volume { get; set; }

        /// <summary>
        /// The quote asset volume traded during this candlestick
        /// </summary>
        [JsonProperty("quote_volume")]
        public decimal QuoteVolume { get; set; }

        public override string ToString()
        {
            return string.Format("{{ Time : {0}, Open : {1}, Close : {2}, High : {3}, Low : {4}, Volume : {5}, QuoteVolume : {6} }}",
                this.Time, this.Open, this.Close, this.High, this.Low, this.Volume, this.QuoteVolume);
        }
    }
}