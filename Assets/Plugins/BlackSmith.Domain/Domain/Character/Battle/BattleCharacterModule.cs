﻿using BlackSmith.Domain.Character.Player;
using BlackSmith.Domain.Item.Equipment;
using Newtonsoft.Json;
using System;

#nullable enable

namespace BlackSmith.Domain.Character.Battle
{
    /// <summary>
    /// 戦闘時のキャラクターのモジュール
    /// </summary>
    public record CharacterBattleModule
    {
        public CharacterLevel Level { get; }

        public HealthPoint HealthPoint { get; }
        public AttackValue Attack { get; }
        public DefenseValue Defense { get; }

        public LevelDependentParameters LevelDependentParameters { get; }

        public BattleEquipmentModule EquipmentModule { get; }
        public BattleStatusEffectModule StatusEffectModule { get; }

        [JsonConstructor]
        internal CharacterBattleModule(HealthPoint healthPoint, LevelDependentParameters levelDependentParameters, BattleEquipmentModule equipmentModule, BattleStatusEffectModule statusEffectModule)
        {
            HealthPoint = healthPoint;
            LevelDependentParameters = levelDependentParameters;
            Level = LevelDependentParameters.Level;
            EquipmentModule = equipmentModule;
            StatusEffectModule = statusEffectModule;

            Attack = new AttackValue(levelDependentParameters, equipmentModule, statusEffectModule);
            Defense = new DefenseValue(levelDependentParameters, equipmentModule, statusEffectModule);
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

            var removed = (item?.EquipType ?? changeType) switch
            {
                EquipmentType.Weapon => EquipmentModule.Weapon,
                EquipmentType.Armor => EquipmentModule.Armor,
                _ => throw new ArgumentException($"Unexpected type of equipment. {item?.EquipType ?? changeType}. (Hi6JnKRG)"),
            };

            return new ChangeEquipmentResult(new CharacterBattleModule(HealthPoint, LevelDependentParameters, currentModule, StatusEffectModule), removed);
        }

        internal record ChangeEquipmentResult
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