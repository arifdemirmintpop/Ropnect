using System;
using Newtonsoft.Json;

namespace tiplay.RemoteExporterTool
{
    public class CustomDoubleConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(double);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            double doubleValue = (double)value;
            if (doubleValue % 1 == 0)
                writer.WriteValue((int)doubleValue);
            else
                writer.WriteValue(doubleValue);
        }
    }
}