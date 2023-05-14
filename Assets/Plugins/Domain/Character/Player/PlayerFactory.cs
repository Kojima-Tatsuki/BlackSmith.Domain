using System;
using BlackSmith.Domain.CharacterObject;

namespace BlackSmith.Domain.Character.Player
{
    // プレイヤー含むキャラクターとして振る舞わせるなら、名前は変えるべき
    /// <summary>
    /// プレイヤーエンティティの再構築を行うオブジェクト
    /// </summary>
    public class PlayerFactory
    {
        /// <summary>
        /// コマンドを用いて再構築を行う
        /// </summary>
        /// <param name="command">使用するコマンド</param>
        /// <returns>再構築したエンティティ</returns>
        public static PlayerEntity Create(PlayerCreateCommand command)
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
        public static PlayerEntity Create(PlayerName name)
        {
            var id = new PlayerID(Guid.NewGuid());
            var levelParams = new PlayerLevelDepentdentParameters();
            var health = new HealthPoint(levelParams.Level);

            var command = new PlayerCreateCommand(id, name, health, levelParams);

            return new PlayerEntity(command);
        }
    }

    // 外部公開するためのプリミティブと、再利用の為の機能が混在している
    // 外部公開のためのオブジェクトが必要かは考えるべき
    // Entityの公開範囲設定で制御する案もある

    /// <summary>プレイヤーの再構築を行う際に引数に指定して使う</summary>
    public record PlayerCreateCommand
    {
        internal PlayerID id { get; }
        internal PlayerName name { get; }
        internal HealthPoint health { get; }
        internal PlayerLevelDepentdentParameters levelParams { get; }

        #region 外部公開するプリミティブ型
        public string Id => id.ToString();
        public string Name => name.Value;
        /// <summary> 体力の現在値と最大値, item1が現在値, item2が最大値 </summary>
        public (int current, int max) Health => health.GetValues();
        public int Exp => levelParams.Level.CumulativeExp.Value;
        public int STR => levelParams.STR.Value;
        public int AGI => levelParams.AGI.Value;
        #endregion

        public PlayerCreateCommand(
            string id,
            string name,
            int currentHealth, int maxHealth,
            int exp, int str, int agi
            )
        {
            this.id = new PlayerID(new Guid(id));
            this.name = new PlayerName(name);

            health = new HealthPoint(
                new HealthPointValue(currentHealth),
                new MaxHealthPointValue(maxHealth));

            levelParams = new PlayerLevelDepentdentParameters(
                new PlayerLevel(new Experience(exp)),
                new Strength(str),
                new Agility(agi));
        }

        internal PlayerCreateCommand(
            PlayerID id,
            PlayerName name,
            HealthPoint health,
            PlayerLevelDepentdentParameters levelParams)
        {
            this.id = id;
            this.name = name;
            this.health = health;
            this.levelParams = levelParams;
        }

        public override string ToString()
        {
            return
                $"ID : {id}\n" +
                $"Name : {Name}\n" +
                $"Health : {Health.current} / {Health.max}\n" +
                $"Exp(Lv) : {Exp}({levelParams.Level.Value})\n" +
                $"STR, AGI : {STR}, {AGI}";
        }
    }
}