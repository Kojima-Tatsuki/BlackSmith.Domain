using BlackSmith.Domain.Character.Interface;
using BlackSmith.Domain.Character.Player;
using BlackSmith.Domain.CharacterObject;
using BlackSmith.Domain.PassiveEffect;
using log4net.Core;
using PlasticGui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        internal IReadOnlyCollection<BattleStatusEffect> StatusEffects => StatusEffectDictionary.Values.ToList();
        private IReadOnlyDictionary<EffectID, BattleStatusEffect> StatusEffectDictionary { get; }

        internal CharacterBattleModule(HealthPoint health, LevelDependentParameters levelDepParams, BattleEquipmentModule equipmentModule, IReadOnlyDictionary<EffectID, BattleStatusEffect>? statusEffects = null)
        {
            HealthPoint = health;
            LevelDependentParameters = levelDepParams;
            Level = LevelDependentParameters.Level;
            EquipmentModule = equipmentModule;
            StatusEffectDictionary = statusEffects ?? new Dictionary<EffectID, BattleStatusEffect>();

            Attack = new AttackValue(levelDepParams, equipmentModule);
            Defense = new DefenseValue(levelDepParams, equipmentModule);
        }

        internal CharacterBattleModule TakeDamage(DamageValue damage)
        {
            var health = HealthPoint.TakeDamage(damage);

            return new CharacterBattleModule(health, LevelDependentParameters, EquipmentModule);
        }

        internal CharacterBattleModule HealHealth(int value)
        {
            var health = HealthPoint.HealHealth(value);

            return new CharacterBattleModule(health, LevelDependentParameters, EquipmentModule);
        }

        internal CharacterBattleModule AddStatusEffect(BattleStatusEffect statusEffect)
        {
            // í«â¡ÇµÇƒê∂ê¨
            var dict = new Dictionary<EffectID, BattleStatusEffect>(StatusEffectDictionary);

            if (dict.Keys.Contains(statusEffect.Id))
                throw new InvalidOperationException($"The effect aleady exists. id: {statusEffect.Id}");

            dict.Add(statusEffect.Id, statusEffect);

            return new CharacterBattleModule(HealthPoint, LevelDependentParameters, EquipmentModule, dict);
        }

        internal CharacterBattleModule RemoveStatusEffect(BattleStatusEffect statusEffect)
        {
            var dict = new Dictionary<EffectID, BattleStatusEffect>(StatusEffectDictionary);

            if (!dict.Keys.Contains(statusEffect.Id))
                throw new InvalidOperationException($"Does not exist the effect. id: {statusEffect.Id}");

            dict.Remove(statusEffect.Id);

            return new CharacterBattleModule(HealthPoint, LevelDependentParameters, EquipmentModule, dict);
        }
    }
}