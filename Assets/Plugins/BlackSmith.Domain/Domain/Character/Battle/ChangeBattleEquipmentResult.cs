using BlackSmith.Domain.Item.Equipment;

#nullable enable

namespace BlackSmith.Domain.Character.Battle
{
    internal record ChangeBattleEquipmentResult
    {
        public EquipableItem? CurrentEquipment { get; }
        public EquipableItem? PrevEquipment { get; }

        internal ChangeBattleEquipmentResult(EquipableItem? currentEquipment, EquipableItem? prevEquipment)
        {
            CurrentEquipment = currentEquipment;
            PrevEquipment = prevEquipment;
        }
    }
}
