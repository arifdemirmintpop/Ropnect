using System;
using Newtonsoft.Json;

namespace tiplay.RemoteExporterTool
{
    public class CustomFloatConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(float);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            float floatValue = (float)value;
            if (floatValue % 1 == 0)
                writer.WriteValue((int)floatValue);
            else
                writer.WriteValue(floatValue);
        }
    }
}