namespace BlackSmith.Domain.Item.Equipment
{
    public record Weapon : EquipableItem
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

    public record WeaponCreateCommand
    {
        public string Name { get; }
        public EquipmentAttack EquipmentAttack { get; }
        public EquipmentDefense EquipmentDefense { get; }
        public EnhancementParameter EnhancementParameter { get; }
        public AdditionalParameter AdditionalParameter { get; }
        public RequireParameter RequireParameter { get; }

        public WeaponCreateCommand(string name, EquipmentAttack attack, EquipmentDefense defense, EnhancementParameter enhancement, AdditionalParameter additional, RequireParameter require)
        {
            Name = name;
            EquipmentAttack = attack;
            EquipmentDefense = defense;
            EnhancementParameter = enhancement;
            AdditionalParameter = additional;
            RequireParameter = require;
        }
    }
}
