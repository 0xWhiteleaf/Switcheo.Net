using CryptoExchange.Net.Converters;
using Newtonsoft.Json;
using Switcheo.Net.Converters;
using System;

namespace Switcheo.Net.Objects
{
    /// <summary>
    /// Details of a asset that is currently under one or more confirming events
    /// </summary>
    public class SwitcheoAssetConfirming
    {
        public class SwitcheoConfirmingEvent
        {
            /// <summary>
            /// The type of event
            /// </summary>
            [JsonProperty("event_type")]
            [JsonConverter(typeof(EventTypeConverter))]
            public EventType Type { get; set; }

            /// <summary>
            /// The concerned asset
            /// </summary>
            [JsonProperty("asset_id")]
            [JsonConverter(typeof(TokenConverter), true)] 
            public SwitcheoToken Asset { get; set; }

            [JsonProperty("amount")]
            private string _Amount { get; set; }

            /// <summary>
            /// Amount of asset involved in this event
            /// </summary>
            [JsonIgnore]
            public decimal Amount
            {
                get
                {
                    return SwitcheoHelpers.FromAssetAmount(this._Amount, this.Asset?.Precision);
                }
                set
                {
                    this._Amount = SwitcheoHelpers.ToAssetAmount(value);
                }
            }

            /// <summary>
            /// Hash of event's transaction (can be null)
            /// </summary>
            [JsonProperty("transaction_hash")]
            public string TransactionHash { get; set; }

            /// <summary>
            /// The creation date of this event (or it's transaction, if not null)
            /// </summary>
            [JsonProperty("created_at")]
            [JsonConverter(typeof(UTCDateTimeConverter))]
            public DateTime CreatedAt { get; set; }

            public override string ToString()
            {
                return string.Format("{{ Type : {0}, Asset : {1}, Amount : {2}, TransactionHash : {3}, CreatedAt : {4} }}", this.Type.ToString(),
                    this.Asset.ToString(), this.Amount, this.TransactionHash, this.CreatedAt.ToString());
            }
        }

        /// <summary>
        /// The asset
        /// </summary>
        public SwitcheoToken Asset { get; set; }

        /// <summary>
        /// Confirming events
        /// </summary>
        public SwitcheoConfirmingEvent[] Events { get; set; }
    }
}
