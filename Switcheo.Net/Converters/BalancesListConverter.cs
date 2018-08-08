using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Switcheo.Net.Helpers;
using Switcheo.Net.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using static Switcheo.Net.Objects.SwitcheoAssetConfirming;

namespace Switcheo.Net.Converters
{
    public class BalancesListConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            SwitcheoBalancesList balancesList = (SwitcheoBalancesList)value;

            writer.WritePropertyName(BalanceType.Confirming.ToString().ToLower());
            writer.WriteStartObject();
            foreach (SwitcheoAssetConfirming assetBalance in balancesList.Confirming)
            {
                writer.WritePropertyName(assetBalance.Asset.GetSymbol());
                writer.WriteStartArray();
                foreach (SwitcheoAssetConfirming.SwitcheoConfirmingEvent confirmingEvent in assetBalance.Events)
                {
                    writer.WriteStartObject();

                    writer.WritePropertyName("event_type");
                    writer.WriteValue(confirmingEvent.Type.ToString().ToLower());
                    writer.WritePropertyName("asset_id");
                    writer.WriteValue(confirmingEvent.Asset.GetId());
                    writer.WritePropertyName("amount");
                    writer.WriteValue(SwitcheoHelpers.ToNeoAssetAmount(confirmingEvent.Amount));
                    writer.WritePropertyName("transaction_hash");
                    writer.WriteValue(confirmingEvent.TransactionHash);
                    writer.WritePropertyName("created_at");
                    writer.WriteValue(confirmingEvent.CreatedAt);

                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
            }
            writer.WriteEndObject();

            writer.WritePropertyName(BalanceType.Confirmed.ToString().ToLower());
            writer.WriteStartObject();
            foreach (SwitcheoAssetBalance assetBalance in balancesList.Confirmed)
            {
                writer.WritePropertyName(assetBalance.Asset.GetSymbol());
                writer.WriteValue(SwitcheoHelpers.ToNeoAssetAmount(assetBalance.Amount));
            }
            writer.WriteEndObject();

            writer.WritePropertyName(BalanceType.Locked.ToString().ToLower());
            writer.WriteStartObject();
            foreach (SwitcheoAssetBalance assetBalance in balancesList.Locked)
            {
                writer.WritePropertyName(assetBalance.Asset.GetSymbol());
                writer.WriteValue(SwitcheoHelpers.ToNeoAssetAmount(assetBalance.Amount));
            }
            writer.WriteEndObject();

            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            SwitcheoBalancesList balancesList = new SwitcheoBalancesList();

            if (reader.TokenType == JsonToken.StartObject)
            {
                JObject rootObject = JObject.Load(reader);

                if (rootObject.ContainsKey(BalanceType.Confirming.ToString().ToLower()))
                {
                    JToken confirmingBalances = rootObject.GetValue(BalanceType.Confirming.ToString().ToLower());

                    balancesList.Confirming = new SwitcheoAssetConfirming[confirmingBalances.Children().Count()];
                    int tabIndex = 0;

                    foreach (JProperty assetConfirming in confirmingBalances.Children())
                    {
                        SwitcheoAssetConfirming _assetConfirming = new SwitcheoAssetConfirming()
                        {
                            Asset = new SupportedAssetConverter().GetSupportedAssetFromString(assetConfirming.Name),
                        };

                        List<SwitcheoConfirmingEvent> events = new List<SwitcheoConfirmingEvent>();
                        foreach (JObject confirmingEvent in (JArray)assetConfirming.Value)
                        {
                            var switcheoConfirmingEvent = JsonConvert.DeserializeObject<SwitcheoConfirmingEvent>(confirmingEvent.ToString());
                            events.Add(switcheoConfirmingEvent);
                        }
                        _assetConfirming.Events = events.ToArray();
                        
                        balancesList.Confirming[tabIndex] = _assetConfirming;
                        tabIndex++;
                    }
                }
                if (rootObject.ContainsKey(BalanceType.Confirmed.ToString().ToLower()))
                {
                    JToken confirmedBalances = rootObject.GetValue(BalanceType.Confirmed.ToString().ToLower());

                    balancesList.Confirmed = new SwitcheoAssetBalance[confirmedBalances.Children().Count()];
                    int tabIndex = 0;

                    foreach (JProperty assetBalance in confirmedBalances.Children())
                    {
                        balancesList.Confirmed[tabIndex] = new SwitcheoAssetBalance()
                        {
                            Asset = new SupportedAssetConverter().GetSupportedAssetFromString(assetBalance.Name),
                            Amount = SwitcheoHelpers.FromNeoAssetAmount(assetBalance.Value.ToString())
                        };
                        tabIndex++;
                    }
                }
                if (rootObject.ContainsKey(BalanceType.Locked.ToString().ToLower()))
                {
                    JToken lockedBalances = rootObject.GetValue(BalanceType.Locked.ToString().ToLower());

                    balancesList.Locked = new SwitcheoAssetBalance[lockedBalances.Children().Count()];
                    int tabIndex = 0;

                    foreach (JProperty assetBalance in lockedBalances.Children())
                    {
                        balancesList.Locked[tabIndex] = new SwitcheoAssetBalance()
                        {
                            Asset = new SupportedAssetConverter().GetSupportedAssetFromString(assetBalance.Name),
                            Amount = SwitcheoHelpers.FromNeoAssetAmount(assetBalance.Value.ToString())
                        };
                        tabIndex++;
                    }
                }
            }

            return balancesList;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(SwitcheoBalancesList);
        }
    }
}
