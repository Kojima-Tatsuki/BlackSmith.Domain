using BlackSmith.Domain.CharacterObject;
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
        internal static PlayerEntity Create(PlayerCreateCommand command)
        {
            if (command is null)
                throw new ArgumentNullException(nameof(command));

            return new PlayerEntity(command);
        }

        /// <summary>
        /// 新たにプレイヤーエンティティを作成する
        /// </summary>
        /// <param name="name">作成するプレイヤーの名前</param>
        /// <returns>作成したエンティティ</returns>
        internal static PlayerEntity Create(PlayerName name)
        {
            var id = new CharacterID();
            var levelParams = new LevelDependentParameters();
            var health = new HealthPoint(levelParams.Level);

            var command = new PlayerCreateCommand(id, name, health, levelParams);

            return new PlayerEntity(command);
        }
    }

    /// <summary>プレイヤーの再構築を行う際に引数に指定して使う</summary>
    internal record PlayerCreateCommand
    {
        internal CharacterID Id { get; }
        internal PlayerName Name { get; }
        internal HealthPoint Health { get; }
        internal LevelDependentParameters LevelParams { get; }

        internal PlayerCreateCommand(
            CharacterID id,
            PlayerName name,
            HealthPoint health,
            LevelDependentParameters levelParams)
        {
            Id = id;
            Name = name;
            Health = health;
            LevelParams = levelParams;
        }

        internal static PlayerCreateCommand BuildWithPrimitive(string id, string name,
            int currentHealth, int maxHealth,
            int exp, int str, int agi
            )
        {
            return new PlayerCreateCommand(
                new CharacterID(id),
                new PlayerName(name),
                new HealthPoint(
                    new HealthPointValue(currentHealth),
                    new MaxHealthPointValue(maxHealth)),
                new LevelDependentParameters(
                    new PlayerLevel(new Experience(exp)),
                    new Strength(str),
                    new Agility(agi)));
        }
    }
}