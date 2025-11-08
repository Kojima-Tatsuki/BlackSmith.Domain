using BlackSmith.Domain.Item.Equipment;

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

        public IOneByInventoryService<EquipableItem> CreateEquipInventory()
        {
            InventoryID id = new InventoryID();

            var inventory = new EquipmentInventory(id);
            return inventory;
        }
    }
}