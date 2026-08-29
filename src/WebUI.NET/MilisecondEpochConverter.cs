using System;
using Newtonsoft.Json;

namespace WebUI
{
    public class MilisecondEpochConverter : RandomNumberTimeConverter<long, DateTime>
    {
        private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public MilisecondEpochConverter() : base(long.Parse) { }

        public static DateTime ParseLong(long val) => _epoch.AddMilliseconds(val);

        protected override DateTime ParseValue(long val) => ParseLong(val);

        protected override string WriteValue(DateTime when)
        {
            return ((long)(when - _epoch).TotalMilliseconds).ToString();
        }
        public override bool CanConvert(Type objectType)
        {
            if (objectType == typeof(DateTime) || objectType == typeof(DateTime?))
            {
                return true;
            }

            if (objectType == typeof(DateTimeOffset) || objectType == typeof(DateTimeOffset?))
            {
                return true;
            }

            return false;
        }
    }
    public abstract class RandomNumberTimeConverter<T, SOURCE_TYPE> : JsonConverter
    {
        protected RandomNumberTimeConverter(Func<String, T> ParseNotNullStringHandler)
        {
            this.ParseNotNullStringHandler = ParseNotNullStringHandler;
        }
        Func<String, T> ParseNotNullStringHandler;

        protected abstract SOURCE_TYPE ParseValue(T val);
        protected abstract String WriteValue(SOURCE_TYPE when);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteRawValue(WriteValue((SOURCE_TYPE)value));


        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null) { return null; }
            T val;
            if (reader.TokenType == JsonToken.String)
                val = ParseNotNullStringHandler((string)reader.Value);
            else
                val = (T)Convert.ChangeType(reader.Value, typeof(T));
            return ParseValue(val);
        }
    }
}
