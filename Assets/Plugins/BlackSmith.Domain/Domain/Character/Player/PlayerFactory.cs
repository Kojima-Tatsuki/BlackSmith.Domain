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
        internal static PlayerCommonEntity Reconstruct(PlayerCommonReconstractCommand command)
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

            var command = new PlayerCommonReconstractCommand(id, name, level);

            return new PlayerCommonEntity(command);
        }
    }

    /// <summary>プレイヤーの再構築を行う際に引数に指定して使う</summary>
    public record PlayerCommonReconstractCommand
    {
        public CharacterID Id { get; }
        public PlayerName Name { get; }
        public PlayerLevel Level { get; }

        internal PlayerCommonReconstractCommand(CharacterID id, PlayerName name, PlayerLevel level)
        {
            Id = id;
            Name = name;
            Level = level;
        }

        public PlayerCommonReconstractCommand(string id, string name, int exp)
        {
            Id = new CharacterID(id);
            Name = new PlayerName(name);
            Level = new PlayerLevel(new Experience(exp));
        }
    }
}