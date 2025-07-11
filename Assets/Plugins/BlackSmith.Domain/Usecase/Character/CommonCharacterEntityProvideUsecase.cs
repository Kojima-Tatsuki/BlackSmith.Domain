﻿using BlackSmith.Domain.Character.Player;

namespace BlackSmith.Usecase.Character
{
    /// <summary>
    /// Presenter層にPlayerEntityDataを渡すクラス
    /// </summary>
    public class CommonCharacterEntityProvideUsecase
    {
        /// <summary>
        /// Commandを用いて再構築を行う, リポジトリへの登録は行わない
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static CommonCharacterEntity BuildCommonEntity(CommonCharacterReconstructCommand command)
        {
            return CommonCharacterFactory.Reconstruct(command);
        }
    }
}
