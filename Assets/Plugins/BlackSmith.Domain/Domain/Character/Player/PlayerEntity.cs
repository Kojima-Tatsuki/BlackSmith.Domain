using BlackSmith.Domain.Character.Player.Event;
using BlackSmith.Domain.Character.Interface;
using BlackSmith.Domain.CharacterObject;
using System;
using BlackSmith.Domain.Character.Battle;
using BlackSmith.Domain.CharacterObject.Interface;

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
        internal PlayerEntity(PlayerCreateCommand command)
        {
            ID = command.id;
            Name = command.name;
            Level = new PlayerLevel(new Experience(command.Exp));
            BattleModule = new CharacterBattleModule(
                new HealthPoint(new(command.Health.current), new(command.Health.max)), 
                command.levelParams, 
                new BattleEquipmentModule(null, null), 
                new BlattleStatusEffectModule());
        }

        /// <summary> 名前の変更を行う </summary>
        /// <param name="name">変更する名前</param>
        void ICharacterEntity.ChangeName(PlayerName name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        #region BattleModule
        private CharacterBattleModule BattleModule { get; set; }

        public HealthPoint HealthPoint => BattleModule.HealthPoint;
        public AttackValue Attack => BattleModule.Attack;
        public DefenseValue Defense => BattleModule.Defense;

        HealthPoint ITakeDamageable.TakeDamage(DamageValue damage)
        {
            BattleModule = BattleModule.TakeDamage(damage);

            return BattleModule.HealthPoint;
        }

        HealthPoint ITakeDamageable.HealHealth(int value)
        {
            BattleModule = BattleModule.HealHealth(value);

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