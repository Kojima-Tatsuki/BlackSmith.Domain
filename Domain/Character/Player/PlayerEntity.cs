using BlackSmith.Domain.Character.Player.Event;
using BlackSmith.Domain.Character.Interface;
using BlackSmith.Domain.CharacterObject;
using System;

namespace BlackSmith.Domain.Character.Player
{
    // Facade, 窓口のようなイメージで扱う
    /// <summary>プレイヤーのエンティティ</summary>
    public class PlayerEntity : ICharacterEntity
    {
        public PlayerID ID { get; }

        /// <summary>プレイヤーの名前</summary>
        public PlayerName Name { get; private protected set; }

        /// <summary>体力</summary>
        public HealthPoint HealthPoint { get; private protected set; }

        // プレイヤーレベルとキャラクターレベルで分ける必要性が感じられない
        // NPCもプレイヤーと同様にレベルアップする仕組みなら問題が発生しない
        ICharacterLevel ICharacterEntity.Level => levelParameters.Level;
        public PlayerLevel Level => levelParameters.Level;
        public AttackValue Attack => new AttackValue(levelParameters);
        public DefenceValue Defence => new DefenceValue(levelParameters);

        private protected PlayerLevelDepentdentParameters levelParameters;

        // イベントの発行を行うためのもの
        // インターフェースで提供されるべき
        private readonly IHealthEventObserver? HealthEventObserver;

        /// <summary>
        /// プレイヤーエンティティのインスタンス化を行う
        /// </summary>
        /// <remarks>再利用する際は、ファクトリーで変更メソッドを呼び出す？</remarks>
        /// <param name="id"></param>
        internal PlayerEntity(PlayerCreateCommand command, IHealthEventObserver? healthEventObserber = null)
        {
            ID = command.id;
            Name = command.name;
            HealthPoint = command.health;
            levelParameters = command.levelParams;

            HealthEventObserver = healthEventObserber;
        }

        /// <summary> 名前の変更を行う </summary>
        /// <param name="name">変更する名前</param>
        public void ChangeName(PlayerName name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public HealthPoint TakeDamage(DamageValue damage)
        {
            HealthPoint = HealthPoint.TakeDamage(damage);

            HealthEventObserver?.SetChangedPlayerHealth(new PlayerHealthChangedEvent(ID, HealthPoint));

            if (HealthPoint.IsDead())
            {
                HealthEventObserver?.SetOnPlayerDead(new PlayerOnDeadEvent(ID));
            }

            return HealthPoint;
        }

        public HealthPoint HealHealth(int value)
        {
            HealthPoint.HealHealth(value);

            HealthEventObserver?.SetChangedPlayerHealth(new PlayerHealthChangedEvent(ID, HealthPoint));

            return HealthPoint;
        }

        /*public IObservable<Experience> AddExpObservable => addExpSubject;
        private Subject<Experience> addExpSubject;
        public void AddExp(Experience exp)
        {

        }*/

        public PlayerCreateCommand GetPlayerCreateCommand()
        {
            return new PlayerCreateCommand(ID, Name, HealthPoint, levelParameters);
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