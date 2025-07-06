using BlackSmith.Domain.Character.Battle;
using System;

namespace BlackSmith.Usecase.Character.Battle
{
    public class PlayerBattleEntityProvideUsecase
    {
        /// <summary>
        /// Commandを用いて再構築を行う, リポジトリへの登録は行わない
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static BattleCharacterEntity BuildBattleEntity(PlayerBattleReconstructCommand command)
        {
            if (command is null) 
                throw new ArgumentNullException(nameof(command));

            return new BattleCharacterEntity(command);
        }
    }
}
