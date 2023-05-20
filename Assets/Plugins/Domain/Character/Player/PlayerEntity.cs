using BlackSmith.Domain.Character.Player.Event;
using BlackSmith.Domain.Character.Interface;
using BlackSmith.Domain.CharacterObject;
using System;
using BlackSmith.Domain.Character.Battle;

namespace BlackSmith.Domain.Character.Player
{
    // Facade, 窓口のようなイメージで扱う
    public class PlayerEntity : ICharacterEntity, IBattleCharacter
    {
        public PlayerID ID { get; }
        public PlayerName Name { get; private set; }

        CharacterLevel ICharacterEntity.Level => Level;
        CharacterLevel IBattleCharacter.Level => Level;
        public PlayerLevel Level { get; private set; }

        /// <summary>
        /// プレイヤーエンティティのインスタンス化を行う
        /// </summary>
        /// <remarks>再利用する際は、ファクトリーで変更メソッドを呼び出す？</remarks>
        /// <param name="id"></param>
        internal PlayerEntity(PlayerCreateCommand command, IHealthEventObserver? healthEventObserver = null)
        {
            ID = command.id;
            Name = command.name;
            Level = new PlayerLevel(new Experience(command.Exp));
            BattleModule = new(command.levelParams.Level, new HealthPoint(new(command.Health.current), new(command.Health.max)), command.levelParams);

            HealthEventObserver = healthEventObserver;
        }

        /// <summary> 名前の変更を行う </summary>
        /// <param name="name">変更する名前</param>
        public void ChangeName(PlayerName name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        #region BattleModule
        private CharacterBattleModule BattleModule { get; set; }

        // イベントの発行を行うためのもの
        // インターフェースで提供されるべき
        private readonly IHealthEventObserver? HealthEventObserver;

        public HealthPoint HealthPoint => BattleModule.HealthPoint;
        public AttackValue Attack => BattleModule.Attack;
        public DefenseValue Defense => BattleModule.Defense;

        public HealthPoint TakeDamage(DamageValue damage)
        {
            BattleModule = BattleModule.TakeDamage(damage);

            HealthEventObserver?.SetChangedPlayerHealth(new PlayerHealthChangedEvent(ID, BattleModule.HealthPoint));

            if (BattleModule.HealthPoint.IsDead())
            {
                HealthEventObserver?.SetOnPlayerDead(new PlayerOnDeadEvent(ID));
            }

            return BattleModule.HealthPoint;
        }

        public HealthPoint HealHealth(int value)
        {
            BattleModule = BattleModule.HealHealth(value);

            HealthEventObserver?.SetChangedPlayerHealth(new PlayerHealthChangedEvent(ID, BattleModule.HealthPoint));

            return BattleModule.HealthPoint;
        }
        #endregion

        /*public IObservable<Experience> AddExpObservable => addExpSubject;
        private Subject<Experience> addExpSubject;
        public void AddExp(Experience exp)
        {

        }*/

        public PlayerCreateCommand GetPlayerCreateCommand()
        {
            return new PlayerCreateCommand(ID, Name, BattleModule.HealthPoint, BattleModule.LevelDependentParameters);
        }

        /// <summary>
        /// 内部情報を文字列として表示する
        /// </summary>
        public override string ToString()
        {
            var result = "";
            result += $"Name : {Name.Value}\n";
            result += $"ID : {ID}\n";
            result += $"Level : {Level.Value}";

            return result;
        }
    }
}