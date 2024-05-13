using BlackSmith.Domain.Character;
using System.Collections.Generic;
using BlackSmith.Domain.Character.Battle;

#nullable enable

namespace BlackSmith.Usecase.Interface
{
    public interface IPlayerBattleEntityRepository
    {
        void Register(PlayerBattleEntity character);

        void UpdateCharacter(PlayerBattleEntity character);

        PlayerBattleEntity? FindByID(CharacterID id);

        IReadOnlyCollection<PlayerBattleEntity> GetAllPlayers();

        bool IsExist(CharacterID id);

        void Delete(CharacterID id);
    }
}
