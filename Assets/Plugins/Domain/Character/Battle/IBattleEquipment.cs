using BlackSmith.Domain.CharacterObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackSmith.Domain.Character.Battle
{
    internal interface IBattleEquipment
    {
        public EquipmentType Type { get; }

        public AttackValue Attack { get; }

        public DefenseValue Defense { get; }
    }

    public enum EquipmentType
    {
        Weapon,
        Armor,
    }
}
