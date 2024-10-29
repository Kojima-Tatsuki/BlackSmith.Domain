using BlackSmith.Domain.Character;
using BlackSmith.Domain.Character.Player;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

#nullable enable

namespace BlackSmith.Usecase.JsonConverters
{
    public class PlayerCommonReconstractCommandJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(PlayerCommonReconstractCommand);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (!CanConvert(objectType))
                throw new JsonSerializationException($"JsonConverter {nameof(PlayerCommonReconstractCommandJsonConverter)} cannot convert {objectType}.");

            var jo = JObject.Load(reader);

            var id = jo["Id"]?.ToObject<CharacterID>(serializer) ?? throw new JsonSerializationException("Json param key \"ID\" is not found.");
            var name = jo["Name"]?.ToObject<PlayerName>(serializer) ?? throw new JsonSerializationException("Json param key \"Name\" is not found.");
            var level = jo["Level"]?.ToObject<PlayerLevel>(serializer) ?? throw new JsonSerializationException("Json param key \"Level\" is not found.");

            return new PlayerCommonReconstractCommand(id, name, level);
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}