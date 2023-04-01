using BlackSmith.Domain.Inventory;

namespace BlackSmith.Usecase.Interface
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