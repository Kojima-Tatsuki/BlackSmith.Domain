using BlackSmith.Domain.Item.Equipment;
using System;

#nullable enable

namespace BlackSmith.Domain.Character.Battle
{
    internal class BattleEquipmentModule
    {
        public EquippableItem? Weapon { get; }
        public EquippableItem? Armor { get; }

        public BattleEquipmentModule(EquippableItem? weapon, EquippableItem? armor)
        {
            if (weapon?.EquipType != EquipmentType.Weapon)
                throw new ArgumentException($"Unexpected type of equipment. {weapon?.EquipType}. (lJTQ1VIJ)");
            if (armor?.EquipType != EquipmentType.Armor)
                throw new ArgumentException($"Unexpected type of equipment. {armor?.EquipType}. (aJL3gEST)");

            Weapon = weapon;
            Armor = armor;
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
                    throw new ArgumentException($"Unexpected type of equipment. {item.EquipType}. (MFXmEum9)");
            }
        }

        public BattleEquipmentModule RemoveEquipment(EquipmentType type)
        {
            switch (type)
            {
                case EquipmentType.Weapon:
                    return new BattleEquipmentModule(null, Armor);
                case EquipmentType.Armor:
                    return new BattleEquipmentModule(Weapon, null);
                default:
                    throw new ArgumentException($"Unexpected type of equipment. {type}. (PfgrJ0iM)");
            }
        }
    }
}
