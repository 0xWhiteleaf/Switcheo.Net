using Newtonsoft.Json;
using Switcheo.Net.Objects;
using System;

namespace Switcheo.Net.Converters
{
    public class TokenConverter : JsonConverter
    {
        private bool qualifiedById;

        public TokenConverter() { this.qualifiedById = false;  }

        public TokenConverter(bool qualifiedById)
        {
            this.qualifiedById = qualifiedById;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            SwitcheoToken token = (SwitcheoToken)value;
            writer.WriteValue(qualifiedById ? token.Id : token.Symbol);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            SwitcheoToken token = null;

            var client = (SwitcheoClient)serializer.Context.Context;

            if (reader.Value != null)
            {
                token = client.GetToken(reader.Value.ToString());
            }

            return token;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(SwitcheoToken);
        }
    }
}
