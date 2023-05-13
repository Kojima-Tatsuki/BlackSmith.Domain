using BlackSmith.Domain.Character.Interface;
using BlackSmith.Domain.Character.Player;
using BlackSmith.Domain.CharacterObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackSmith.Domain.Character.Battle
{
    internal class CharacterBattleModule
    {
        internal ICharacterLevel Level { get; }

        internal HealthPoint HealthPoint { get; }

        internal PlayerLevelDependentParameters LevelDependentParameters { get; }

        internal AttackValue Attack { get; }
        internal DefenseValue Defense { get; }

        internal CharacterBattleModule(ICharacterLevel level, HealthPoint health, PlayerLevelDependentParameters levelDepParams)
        {
            Level = level;
            HealthPoint = health;
            LevelDependentParameters = levelDepParams;
            Attack = new AttackValue(levelDepParams);
            Defense = new DefenseValue(levelDepParams);
        }

        internal CharacterBattleModule TakeDamage(DamageValue damage)
        {
            var health = HealthPoint.TakeDamage(damage);

            return new CharacterBattleModule(Level, health, LevelDependentParameters);
        }

        internal CharacterBattleModule HealHealth(int value)
        {
            var health = HealthPoint.HealHealth(value);

            return new CharacterBattleModule(Level, health, LevelDependentParameters);
        }
    }
}
