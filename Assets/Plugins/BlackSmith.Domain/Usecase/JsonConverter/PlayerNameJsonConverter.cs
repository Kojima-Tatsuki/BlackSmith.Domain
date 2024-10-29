using BlackSmith.Domain.Character.Player;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

#nullable enable

namespace BlackSmith.Usecase.JsonConverters
{
    public class PlayerNameJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
           return objectType == typeof(PlayerName);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (!CanConvert(objectType))
                throw new JsonSerializationException($"JsonConverter {nameof(PlayerNameJsonConverter)} cannot convert {objectType}.");

            var jo = JObject.Load(reader);

            var value = jo["Value"]?.Value<string>() ?? throw new JsonSerializationException("Json param key \"Value\" is not found.");

            return new PlayerName(value);
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}