using BlackSmith.Domain.Character;
using BlackSmith.Domain.Character.Player;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;

#nullable enable

namespace BlackSmith.Usecase.JsonConverters
{
    public class CharacterLevelJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(CharacterLevel);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (!CanConvert(objectType))
                throw new JsonSerializationException($"JsonConverter {nameof(CharacterLevelJsonConverter)} cannot convert {objectType}.");

            var jo = JObject.Load(reader);

            var value = jo["Value"]?.Value<int>() ?? throw new JsonSerializationException("Json param key \"Value\" is not found.");

            return new CharacterLevel(value);
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public class ExperienceJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Experience);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (!CanConvert(objectType))
                throw new JsonSerializationException($"JsonConverter {nameof(ExperienceJsonConverter)} cannot convert {objectType}.");

            var jo = JObject.Load(reader);

            var value = jo["Value"]?.Value<int>() ?? throw new JsonSerializationException("Json param key \"Value\" is not found.");

            return new Experience(value);
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public class PlayerLevelJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(PlayerLevel);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (!CanConvert(objectType))
                throw new JsonSerializationException($"JsonConverter {nameof(PlayerLevelJsonConverter)} cannot convert {objectType}.");

            var jo = JObject.Load(reader);

            var exp = jo["CumulativeExp"]?.ToObject<Experience>(serializer) ?? throw new JsonSerializationException("Json param key \"Value\" is not found.");

            return new PlayerLevel(exp);
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}