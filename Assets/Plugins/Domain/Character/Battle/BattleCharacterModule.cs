using BlackSmith.Domain.Character.Interface;
using BlackSmith.Domain.Character.Player;
using BlackSmith.Domain.CharacterObject;
using log4net.Core;
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

        internal CharacterBattleModule(HealthPoint health, LevelDependentParameters levelDepParams, BattleEquipmentModule equipmentModule)
        {
            HealthPoint = health;
            LevelDependentParameters = levelDepParams;
            Level = LevelDependentParameters.Level;
            EquipmentModule = equipmentModule;

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
    }
}