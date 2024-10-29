using BlackSmith.Domain.Character.Player;
using BlackSmith.Usecase.JsonConverters;
using Newtonsoft.Json;

namespace BlackSmith.Usecase.Character
{
    /// <summary>
    /// Presenter層にPlayerEntityDataを渡すクラス
    /// </summary>
    public class PlayerCommonEntityProvidUsecase
    {
        /// <summary>
        /// Commandを用いて再構築を行う, リポジトリへの登録は行わない
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static PlayerCommonEntity BuildCommonEntity(PlayerCommonReconstractCommand command)
        {
            return PlayerFactory.Reconstruct(command);
        }

        public static string Serialize(PlayerCommonReconstractCommand command)
        {
            return JsonConvert.SerializeObject(command);
        }

        public static PlayerCommonReconstractCommand Deserialize(string json)
        {
            var commandJsonConverters = new JsonConverter[]
{
                new PlayerCommonReconstractCommandJsonConverter(),
                new CharacterIDJsonConverter(),
                new PlayerNameJsonConverter(),
                new PlayerLevelJsonConverter(),
                new ExperienceJsonConverter()
            };
            return JsonConvert.DeserializeObject<PlayerCommonReconstractCommand>(json, commandJsonConverters);
        }
    }
}
