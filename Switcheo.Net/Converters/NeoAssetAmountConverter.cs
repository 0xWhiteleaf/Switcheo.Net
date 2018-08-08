using Newtonsoft.Json;
using System;

namespace Switcheo.Net.Converters
{
    public class NeoAssetAmountConverter : JsonConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
                return null;

            return SwitcheoHelpers.FromNeoAssetAmount(reader.Value.ToString());
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(SwitcheoHelpers.ToNeoAssetAmount((decimal)value));
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(decimal);
        }
    }
}
