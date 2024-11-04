using System;

namespace BlackSmith.Domain.Character.Player
{
    // プレイヤー含むキャラクターとして振る舞わせるなら、名前は変えるべき
    /// <summary>
    /// プレイヤーエンティティの再構築を行うオブジェクト
    /// </summary>
    internal class PlayerFactory
    {
        /// <summary>
        /// コマンドを用いて再構築を行う
        /// </summary>
        /// <param name="command">使用するコマンド</param>
        /// <returns>再構築したエンティティ</returns>
        internal static PlayerCommonEntity Reconstruct(PlayerCommonReconstructCommand command)
        {
            if (command is null)
                throw new ArgumentNullException(nameof(command));

            return new PlayerCommonEntity(command);
        }

        /// <summary>
        /// 新たにプレイヤーエンティティを作成する
        /// </summary>
        /// <param name="name">作成するプレイヤーの名前</param>
        /// <returns>作成したエンティティ</returns>
        internal static PlayerCommonEntity Create(PlayerName name)
        {
            var id = new CharacterID();
            var level = new PlayerLevel();

            var command = new PlayerCommonReconstructCommand(id, name, level);

            return Reconstruct(command);
        }
    }
}