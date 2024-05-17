using BlackSmith.Domain.CharacterObject;
using BlackSmith.Domain.CharacterObject.Interface;
using BlackSmith.Domain.Item.Equipment;
using System;

namespace BlackSmith.Domain.Character.Battle
{
    public class PlayerBattleEntity : IBattleCharacter, IEquatable<PlayerBattleEntity>
    {
        public CharacterID ID { get; }

        internal CharacterBattleModule BattleModule { get; set; }

        public CharacterLevel Level => BattleModule.Level;
        public HealthPoint HealthPoint => BattleModule.HealthPoint;
        public AttackValue Attack => BattleModule.Attack;
        public DefenseValue Defense => BattleModule.Defense;

        internal PlayerBattleEntity(PlayerBattleReconstractCommand command)
        {
            ID = command.Id;
            BattleModule = command.Module;
        }

        // スコープをinternalとしたいため、Interfaceのメソッドを明示的に実装
        HealthPoint ITakeDamageable.TakeDamage(DamageValue damage)
        {
            if (damage is null) throw new ArgumentNullException(nameof(damage));

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

        public bool Equals(PlayerBattleEntity other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return ID.Equals(other.ID);
        }
    }

    internal record PlayerBattleReconstractCommand
    {
        public CharacterID Id { get; }
        public CharacterBattleModule Module { get; }

        public PlayerBattleReconstractCommand(CharacterID id, CharacterBattleModule module)
        {
            Id = id;
            Module = module;
        }
    }
}
