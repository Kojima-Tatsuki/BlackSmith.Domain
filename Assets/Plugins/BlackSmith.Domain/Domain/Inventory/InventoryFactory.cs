using BlackSmith.Domain.Item.Equipment;
using System;

namespace BlackSmith.Domain.Inventory
{
    public class InventoryFactory
    {
        public IInventoryService Create()
        {
            InventoryID id = new InventoryID();

            var inventory = new InfiniteSlotInventory(id);
            return inventory;
        }

        public IOneByInventoryService<EquippableItem> CreateEquipInventory()
        {
            InventoryID id = new InventoryID();

            var inventory = new EquipmentInventory(id);
            return inventory;
        }
    }
}