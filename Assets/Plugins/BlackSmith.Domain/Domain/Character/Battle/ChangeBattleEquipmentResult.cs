using BlackSmith.Domain.Item.Equipment;

namespace BlackSmith.Domain.Character.Battle
{
    internal class ChangeBattleEquipmentResult
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
