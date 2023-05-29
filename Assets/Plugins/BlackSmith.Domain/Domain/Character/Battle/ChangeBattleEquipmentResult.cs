using BlackSmith.Domain.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
