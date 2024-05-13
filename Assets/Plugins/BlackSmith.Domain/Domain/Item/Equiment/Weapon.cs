using BlackSmith.Domain.Item.Equipment;

namespace BlackSmith.Domain.Item.Equiment
{
    public record Weapon : EquippableItem
    {
        // 要らない？
        internal Weapon(WeaponCreateCommand command) 
            : base(new CreateCommand(command.Name, EquipmentType.Weapon,
                command.EquipmentAttack, 
                command.EquipmentDefense,
                command.EnhancementParameter,
                command.AdditionalParameter,
                command.RequireParameter))
        {
            
        }
    }

    public record WeaponCreateCommand(
        string Name,
        EquipmentAttack EquipmentAttack,
        EquipmentDefense EquipmentDefense,
        EnhancementParameter EnhancementParameter,
        AdditionalParameter AdditionalParameter,
        RequireParameter RequireParameter);
}
