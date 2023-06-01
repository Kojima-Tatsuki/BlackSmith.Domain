using BlackSmith.Domain.Item;
using BlackSmith.Domain.Item.Equipment;
using System;

namespace BlackSmith.Domain.Character.Battle
{
    internal class BattleEquipmentModule
    {
        public EquippableItem? Weapon { get; }
        public EquippableItem? Armor { get; }

        public BattleEquipmentModule(EquippableItem? weapon, EquippableItem? armor)
        {
            Weapon = weapon is null || weapon.EquipType == EquipmentType.Weapon ? weapon : throw new ArgumentException(nameof(weapon));
            Armor = armor is null || armor.EquipType == EquipmentType.Armor ? armor : throw new ArgumentException(nameof(armor));
        }

        public BattleEquipmentModule ChangeEquipment(EquippableItem item)
        {
            switch (item.EquipType)
            {
                case EquipmentType.Weapon:
                    return new BattleEquipmentModule(item, Armor);
                case EquipmentType.Armor:
                    return new BattleEquipmentModule(Weapon, item);
                default:
                    throw new ArgumentException(nameof(item));
            }
        }

        public BattleEquipmentModule RemoveEquipment(EquipmentType type) {
            switch (type)
            {
                case EquipmentType.Weapon:
                    return new BattleEquipmentModule(null, Armor);
                case EquipmentType.Armor:
                    return new BattleEquipmentModule(Weapon, null);
                default:
                    throw new ArgumentException(nameof(type));
            }
        }
    }
}
