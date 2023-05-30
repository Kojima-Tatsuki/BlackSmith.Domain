using BlackSmith.Domain.CharacterObject;
using BlackSmith.Domain.Item.Equipment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackSmith.Domain.Character.Battle
{
    internal interface IEquippableItem
    {
        public EquipmentType Type { get; }

        public AttackValue Attack { get; }

        public DefenseValue Defense { get; }
    }
}
