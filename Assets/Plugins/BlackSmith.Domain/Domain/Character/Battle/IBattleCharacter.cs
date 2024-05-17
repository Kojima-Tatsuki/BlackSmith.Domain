using BlackSmith.Domain.CharacterObject;
using BlackSmith.Domain.CharacterObject.Interface;

namespace BlackSmith.Domain.Character.Battle
{
    internal interface IBattleCharacter : ITakeDamageable
    {
        CharacterLevel Level { get; }

        HealthPoint HealthPoint { get; }

        AttackValue Attack { get; }
        DefenseValue Defense { get; }
    }
}