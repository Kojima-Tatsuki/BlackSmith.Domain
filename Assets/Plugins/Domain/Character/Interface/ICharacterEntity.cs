using System.Collections;
using System.Collections.Generic;
using BlackSmith.Domain.CharacterObject;
using BlackSmith.Domain.CharacterObject.Interface;

namespace BlackSmith.Domain.Character.Interface
{
    public interface ICharacterEntity : ITakeDamageable
    {
        public HealthPoint HealthPoint { get; }

        public ICharacterLevel Level { get; }
        public AttackValue Attack { get; }
        public DefenceValue Defence { get; }
    }
}
