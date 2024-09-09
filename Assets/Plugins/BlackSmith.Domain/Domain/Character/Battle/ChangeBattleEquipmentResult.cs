using BlackSmith.Domain.Item.Equipment;

#nullable enable

namespace BlackSmith.Domain.Character.Battle
{
    internal record ChangeBattleEquipmentResult
    {
        public EquippableItem? CurrentEquipment { get; }
        public EquippableItem? PrevEquipment { get; }

        internal ChangeBattleEquipmentResult(EquippableItem? currentEquipment, EquippableItem? prevEquipment)
        {
            CurrentEquipment = currentEquipment;
            PrevEquipment = prevEquipment;
        }
    }
}
