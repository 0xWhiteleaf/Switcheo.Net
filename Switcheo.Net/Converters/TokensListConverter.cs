using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Switcheo.Net.Helpers;
using Switcheo.Net.Objects;
using System;
using System.Linq;

namespace Switcheo.Net.Converters
{
    public class TokensListConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            SwitcheoTokensList tokensList = (SwitcheoTokensList)value;

            foreach (SwitcheoToken token in tokensList.Tokens)
            {
                writer.WritePropertyName(token.Symbol);

                writer.WriteStartObject();
                writer.WritePropertyName("hash"); writer.WriteValue(token.Id);
                writer.WritePropertyName("decimals"); writer.WriteValue(token.Precision);
                writer.WriteEndObject();
            }

            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            SwitcheoTokensList tokensList = new SwitcheoTokensList();

            if (reader.TokenType == JsonToken.StartObject)
            {
                JObject rootObject = JObject.Load(reader);

                tokensList.Tokens = new SwitcheoToken[rootObject.Children().Count()];
                int tabIndex = 0;

                foreach (JProperty token in rootObject.Children())
                {
                    var tokenName = token.Name;
                    var tokenInfos = (JObject)token.Value;

                    var tokenHash = tokenInfos.GetValue("hash");
                    var tokenDecimals = tokenInfos.GetValue("decimals");

                    tokensList.Tokens[tabIndex] = new SwitcheoToken()
                    {
                        Symbol = tokenName,
                        Id = (string)tokenHash,
                        Precision = !tokenDecimals.IsNullOrEmpty() ? (int)tokenDecimals : -1
                    };
                    tabIndex++;
                }    
            }

            return tokensList;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(SwitcheoTokensList);
        }
    }
}
