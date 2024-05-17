using BlackSmith.Domain.CharacterObject;
using BlackSmith.Domain.Item.Equipment;

namespace BlackSmith.Domain.Character.Battle
{
    internal interface IEquippableItem
    {
        public EquipmentType Type { get; }

        public AttackValue Attack { get; }

        public DefenseValue Defense { get; }
    }
}
