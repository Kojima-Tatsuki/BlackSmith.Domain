using BlackSmith.Domain.Character.Player;
using System.Collections.Generic;

namespace BlackSmith.Usecase.Interface
{
    /// <summary>
    /// すべてのプレイヤーエンティティを保管するリポジトリ
    /// </summary>
    public interface IPlayerRepository
    {
        void Register(PlayerEntity character);

        void UpdateCharacter(PlayerEntity character);

        PlayerEntity? FindByID(PlayerID id);

        IReadOnlyCollection<PlayerEntity> GetAllPlayers();

        bool IsExist(PlayerID id);

        void Delete(PlayerID id);
    }
}
