using BlackSmith.Domain.CharacterObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackSmith.Domain.Character.Battle
{
    internal class BattleEquipmentModule
    {
        public AttackValue Attack { get; }
        public DefenseValue Defense { get; }

        public IBattleEquipment? Weapon { get; }
        public IBattleEquipment? Armor { get; }

        public BattleEquipmentModule(IBattleEquipment? weapon, IBattleEquipment? armor)
        {
            Weapon = weapon.Type == EquipmentType.Weapon ? weapon : throw new ArgumentException(nameof(weapon));
            Armor = armor.Type == EquipmentType.Armor ? armor : throw new ArgumentException(nameof(armor));
            Attack = Weapon?.Attack + Armor?.Attack ?? new AttackValue(0);
            Defense = Weapon?.Defense + Armor?.Defense ?? new DefenseValue(0);
        }
    }
}
