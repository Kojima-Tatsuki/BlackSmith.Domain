using BlackSmith.Domain.Character.Battle;
using BlackSmith.Domain.Character.Interface;
using BlackSmith.Domain.CharacterObject;
using BlackSmith.Domain.CharacterObject.Interface;
using BlackSmith.Domain.Item.Equipment;
using System;

namespace BlackSmith.Domain.Character.Player
{
    // Facade, 窓口のようなイメージで扱う
    public class PlayerEntity : ICharacterEntity, IBattleCharacter
    {
        public CharacterID ID { get; }
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
            ID = command.Id;
            Name = command.Name;
            Level = command.LevelParams.Level;
            BattleModule = new CharacterBattleModule(
                command.Health,
                command.LevelParams,
                new BattleEquipmentModule(null, null),
                new BlattleStatusEffectModule());
        }

        /// <summary> 名前の変更を行う </summary>
        /// <param name="name">変更する名前</param>
        void ICharacterEntity.ChangeName(PlayerName name)
        {
            Name = name ?? throw new ArgumentNullException("Not found PlayerName. (O94YoFRG)");
        }

        #region BattleModule
        internal CharacterBattleModule BattleModule { get; set; }

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

        // 現状の変更結果は、外部から観測できる範囲に限られるため、Resultクラスの必要性が無い
        internal ChangeBattleEquipmentResult ChangeBattleEquipment(EquippableItem item)
        {
            var prev = item.EquipType switch
            {
                EquipmentType.Weapon => BattleModule.EquipmentModule.Weapon,
                EquipmentType.Armor => BattleModule.EquipmentModule.Armor,
                _ => throw new ArgumentException($"Unexpected type of equipment. {item.EquipType} (SaIz8NCE)")
            };

            BattleModule = BattleModule.ChangeBattleEquipment(item, item.EquipType).Modeule;

            return new ChangeBattleEquipmentResult(item, prev);
        }

        internal ChangeBattleEquipmentResult RemoveBattleEquipment(EquipmentType removeType)
        {
            var prev = removeType switch
            {
                EquipmentType.Weapon => BattleModule.EquipmentModule.Weapon,
                EquipmentType.Armor => BattleModule.EquipmentModule.Armor,
                _ => throw new ArgumentException($"Unexpected type of equipment. {removeType} (oOySNBXC2)")
            };

            BattleModule = BattleModule.ChangeBattleEquipment(null, removeType).Modeule;

            return new ChangeBattleEquipmentResult(null, prev);
        }
        #endregion

        internal PlayerCreateCommand GetPlayerCreateCommand()
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