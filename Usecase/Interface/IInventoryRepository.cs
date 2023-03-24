using BlackSmith.Domain.Inventory;

#nullable enable

namespace BlackSmith.Repository.Interface
{
    public interface IInventoryRepository
    {
        void Register(InventoryID id, IInventoryService inventory);

        void UpdateInventory(InventoryID id, IInventoryService inventory);

        IInventoryService? FindByID(InventoryID id);

        bool IsExist(InventoryID id);

        void Delete(InventoryID id);
    }
}