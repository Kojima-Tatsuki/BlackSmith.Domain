using BlackSmith.Domain.Character.Interface;
using BlackSmith.Domain.Character.Player;
using BlackSmith.Domain.CharacterObject;
using BlackSmith.Domain.CharacterObject.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackSmith.Domain.Character.Battle
{
    public interface IBattleCharacter : ITakeDamageable
    {
        ICharacterLevel Level { get; }

        HealthPoint HealthPoint { get; }

        AttackValue Attack { get; }
        DefenseValue Defense { get; }
    }
}
