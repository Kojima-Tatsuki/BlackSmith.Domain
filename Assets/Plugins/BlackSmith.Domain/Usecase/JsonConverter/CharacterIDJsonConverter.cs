using BlackSmith.Domain.Character;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

#nullable enable

namespace BlackSmith
{
    public class CharacterIDJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(CharacterID);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (!CanConvert(objectType))
                throw new JsonSerializationException($"JsonConverter {nameof(CharacterIDJsonConverter)} cannot convert {objectType}.");

            JObject jo = JObject.Load(reader);

            string? value = jo["Value"]?.Value<string>() ?? throw new JsonSerializationException("Json param key \"Value\" is not found.");

            var id = new CharacterID(value);

            return id;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
