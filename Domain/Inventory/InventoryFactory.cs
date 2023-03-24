using System;
using BlackSmith.Domain.Inventory;
using BlackSmith.Domain.Item;

namespace BlackSmith.Usecase.Inventory
{
    public class InventoryFactory
    {
        public IInventoryService Create()
        {
            InventoryID id = new InventoryID(Guid.NewGuid());

            var inventory = new InfiniteSlotInventory(id);
            return inventory;
        }

        public IOneByInventoryService<EquippableItem> CreateEquipInventory()
        {
            InventoryID id = new InventoryID(Guid.NewGuid());

            var inventory = new EquipmentInventory(id);
            return inventory;
        }
    }
}