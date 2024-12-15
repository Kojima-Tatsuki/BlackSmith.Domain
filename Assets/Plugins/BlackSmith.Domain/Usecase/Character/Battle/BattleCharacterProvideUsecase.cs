using BlackSmith.Domain.Character.Battle;
using System;

namespace BlackSmith.Usecase.Character.Battle
{
    public class BattleCharacterProvideUsecase
    {
        public static PlayerBattleEntity BuildBattleEntity(PlayerBattleReconstractCommand command)
        {
            if (command is null) 
                throw new ArgumentNullException(nameof(command));

            return new PlayerBattleEntity(command);
        }
    }
}
