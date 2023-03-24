using System.Collections.Generic;
using BlackSmith.Domain.Player;

#nullable enable

namespace BlackSmith.Repository.Interface
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

    /// <summary>
    /// ゲーム中のプレイヤーのIDを保管するリポジトリ
    /// </summary>
    public interface ISessionPlayerIdRepository
    {
        void UpdateId(PlayerID id);

        PlayerID? GetId();

        void Logout(PlayerID id);
    }
}
