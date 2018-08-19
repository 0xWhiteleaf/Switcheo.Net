using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Switcheo.Net.Objects;
using System;
using System.Collections.Generic;

namespace Switcheo.Net.Converters
{
    public class PricesListConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            SwitcheoPricesList pricesList = (SwitcheoPricesList)value;

            if (pricesList != null && pricesList.Prices != null && pricesList.Prices.Length > 0)
            {
                Dictionary<string, List<KeyValuePair<string, decimal>>> groupedAssetsPrices = new Dictionary<string, List<KeyValuePair<string, decimal>>>();
                foreach (SwitcheoPrice switcheoPrice in pricesList.Prices)
                {
                    string fromAsset = switcheoPrice.Pair.GetOfferAsset();
                    var toAssetInformations = new KeyValuePair<string, decimal>(switcheoPrice.Pair.GetWantAsset(), switcheoPrice.Price);

                    if (!groupedAssetsPrices.ContainsKey(fromAsset))
                        groupedAssetsPrices.Add(fromAsset, new List<KeyValuePair<string, decimal>>());

                    groupedAssetsPrices[fromAsset].Add(toAssetInformations);
                }

                foreach(var assetPrices in groupedAssetsPrices)
                {
                    writer.WritePropertyName(assetPrices.Key);
                    writer.WriteStartObject();

                    foreach(var subAssetPrice in assetPrices.Value)
                    {
                        writer.WritePropertyName(subAssetPrice.Key);
                        writer.WriteValue(subAssetPrice.Value);
                    }

                    writer.WriteEndObject();
                }
            }

            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            SwitcheoPricesList pricesList = new SwitcheoPricesList();

            if (reader.TokenType == JsonToken.StartObject)
            {
                JObject rootObject = JObject.Load(reader);

                List<SwitcheoPrice> prices = new List<SwitcheoPrice>();

                foreach (var assetPrices in rootObject)
                {
                    foreach (JProperty subAssetPrice in assetPrices.Value)
                    {
                        SwitcheoPrice switcheoPrice = new SwitcheoPrice()
                        {
                            Pair = $"{assetPrices.Key}{SwitcheoHelpers.PairSeparator}{subAssetPrice.Name}",
                            Price = (decimal)subAssetPrice.Value
                        };

                        prices.Add(switcheoPrice);
                    }
                }

                pricesList.Prices = prices.ToArray();
            }

            return pricesList;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(SwitcheoPricesList);
        }
    }
}
