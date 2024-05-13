using BlackSmith.Domain.Character;
using BlackSmith.Domain.Character.Player;
using System.Collections.Generic;

#nullable enable

namespace BlackSmith.Usecase.Interface
{
    /// <summary>
    /// すべてのプレイヤーエンティティを保管するリポジトリ
    /// </summary>
    public interface IPlayerCommonEntityRepository
    {
        void Register(PlayerCommonEntity character);

        void UpdateCharacter(PlayerCommonEntity character);

        PlayerCommonEntity? FindByID(CharacterID id);

        IReadOnlyCollection<PlayerCommonEntity> GetAllPlayers();

        bool IsExist(CharacterID id);

        void Delete(CharacterID id);
    }
}
