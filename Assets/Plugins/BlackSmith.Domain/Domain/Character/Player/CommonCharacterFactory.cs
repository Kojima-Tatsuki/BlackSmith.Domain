using System;

namespace BlackSmith.Domain.Character.Player
{
    // キャラクターとして振る舞わせるなら、名前は変えるべき
    /// <summary>
    /// キャラクターエンティティの再構築を行うオブジェクト
    /// </summary>
    internal class CommonCharacterFactory
    {
        /// <summary>
        /// コマンドを用いて再構築を行う
        /// </summary>
        /// <param name="command">使用するコマンド</param>
        /// <returns>再構築したエンティティ</returns>
        internal static CommonCharacterEntity Reconstruct(CommonCharacterReconstructCommand command)
        {
            if (command is null)
                throw new ArgumentNullException(nameof(command));

            return new CommonCharacterEntity(command);
        }

        /// <summary>
        /// 新たにキャラクターエンティティを作成する
        /// </summary>
        /// <param name="name">作成するキャラクターの名前</param>
        /// <returns>作成したエンティティ</returns>
        internal static CommonCharacterEntity Create(CharacterName name)
        {
            var id = new CharacterID();
            var level = new CharacterLevel();

            var command = new CommonCharacterReconstructCommand(id, name, level);

            return Reconstruct(command);
        }
    }
}