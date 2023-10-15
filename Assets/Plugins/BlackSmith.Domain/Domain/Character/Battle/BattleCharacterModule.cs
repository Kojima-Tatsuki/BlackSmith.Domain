using BlackSmith.Domain.Character.Player;
using BlackSmith.Domain.CharacterObject;
using BlackSmith.Domain.Item;
using BlackSmith.Domain.Item.Equipment;
using System;

namespace BlackSmith.Domain.Character.Battle
{
    internal class CharacterBattleModule
    {
        internal CharacterLevel Level { get; }

        internal HealthPoint HealthPoint { get; }
        internal AttackValue Attack { get; }
        internal DefenseValue Defense { get; }

        internal LevelDependentParameters LevelDependentParameters { get; }

        internal BattleEquipmentModule EquipmentModule { get; }
        internal BlattleStatusEffectModule StatusEffectModule { get; }

        internal CharacterBattleModule(HealthPoint health, LevelDependentParameters levelDepParams, BattleEquipmentModule equipmentModule, BlattleStatusEffectModule statusEffectModule)
        {
            HealthPoint = health;
            LevelDependentParameters = levelDepParams;
            Level = LevelDependentParameters.Level;
            EquipmentModule = equipmentModule;
            StatusEffectModule = statusEffectModule;

            Attack = new AttackValue(levelDepParams, equipmentModule, statusEffectModule);
            Defense = new DefenseValue(levelDepParams, equipmentModule, statusEffectModule);
        }

        internal CharacterBattleModule TakeDamage(DamageValue damage)
        {
            var health = HealthPoint.TakeDamage(damage);

            return new CharacterBattleModule(health, LevelDependentParameters, EquipmentModule, StatusEffectModule);
        }

        internal CharacterBattleModule HealHealth(int value)
        {
            var health = HealthPoint.HealHealth(value);

            return new CharacterBattleModule(health, LevelDependentParameters, EquipmentModule, StatusEffectModule);
        }

        internal ChangeEquipmentResult ChangeBattleEquipment(EquippableItem? item, EquipmentType changeType)
        {
            var currentModule = item is null ? EquipmentModule.RemoveEquipment(changeType) : EquipmentModule.ChangeEquipment(item);

            var removed = (item?.EquipType ?? changeType)  switch
            {
                EquipmentType.Weapon => EquipmentModule.Weapon,
                EquipmentType.Armor => EquipmentModule.Armor,
                _ => throw new ArgumentException($"Unexpected type of equipment. {item?.EquipType ?? changeType}. (Hi6JnKRG)"),
            };

            return new ChangeEquipmentResult(new CharacterBattleModule(HealthPoint, LevelDependentParameters, currentModule, StatusEffectModule), removed);
        }

        internal class ChangeEquipmentResult
        {
            public CharacterBattleModule Modeule { get; }
            public EquippableItem? RemovedItem { get; }

            internal ChangeEquipmentResult(CharacterBattleModule module, EquippableItem? item)
            {
                Modeule = module;
                RemovedItem = item;
            }
        }
    }
}