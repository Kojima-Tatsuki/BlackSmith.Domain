using BlackSmith.Domain.Inventory;
using BlackSmith.Domain.Item;
using BlackSmith.Usecase.Interface;

namespace BlackSmith.Usecase.Inventory
{
    public interface IInventoryCreateUsecase
    {
        /// <summary>
        /// 新たにインベントリを生成する
        /// </summary>
        /// <returns></returns>
        IInventoryService CreateInventory();
    }

    /// <summary>
    /// インベントリを扱う際のユースケース
    /// </summary>
    public class InventoryUsecase : IInventoryCreateUsecase
    {
        private readonly IInventoryRepository inventoryRepository;
        private readonly InventoryFactory inventoryFactory;

        public InventoryUsecase(IInventoryRepository inventoryRepository)
        {
            this.inventoryRepository = inventoryRepository;
            inventoryFactory = new InventoryFactory();
        }

        IInventoryService IInventoryCreateUsecase.CreateInventory()
        {
            return inventoryFactory.Create();
        }

        public void AddItemToInventory(IInventoryService inventory, Item item, int count)
        {
            inventory.AddItem(item, count);
        }
    }
}