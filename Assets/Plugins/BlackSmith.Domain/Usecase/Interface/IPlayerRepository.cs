using BlackSmith.Domain.Character;
using BlackSmith.Domain.Character.Player;
using System.Collections.Generic;

#nullable enable

namespace BlackSmith.Usecase.Interface
{
    /// <summary>
    /// すべてのプレイヤーエンティティを保管するリポジトリ
    /// </summary>
    public interface IPlayerRepository
    {
        void Register(PlayerEntity character);

        void UpdateCharacter(PlayerEntity character);

        PlayerEntity? FindByID(CharacterID id);

        IReadOnlyCollection<PlayerEntity> GetAllPlayers();

        bool IsExist(CharacterID id);

        void Delete(CharacterID id);
    }
}
