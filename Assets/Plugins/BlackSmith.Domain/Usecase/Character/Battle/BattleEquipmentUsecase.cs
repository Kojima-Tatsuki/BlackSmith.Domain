using BlackSmith.Domain.Character.Player;
using BlackSmith.Domain.Inventory;
using BlackSmith.Domain.Item;
using BlackSmith.Usecase.Interface;
using System;

namespace BlackSmith.Usecase.Character.Battle
{
    // Usecase層内部のクラスの可能性あり
    internal class BattleEquipmentUsecase
    {
        private IPlayerRepository PlayerRepository { get; }
        private IInventoryRepository InventoryRepository { get; }

        public BattleEquipmentUsecase(IPlayerRepository playerRepository, IInventoryRepository inventoryRepository)
        {
            PlayerRepository = playerRepository;
            InventoryRepository = inventoryRepository;
        }

        public void ChengeEquipment(PlayerID playerid, InventoryID inventoryId, EquippableItem equipment, EquippableItem remove)
        {
            var player = PlayerRepository.FindByID(playerid) ?? throw new InvalidOperationException(nameof(playerid));
            var inventoryService = InventoryRepository.FindByID(inventoryId) ?? throw new InvalidOperationException(nameof(inventoryId));

            var inventory = inventoryService as EquipmentInventory;

            EquippableItem? removed = null;

            if (!inventory.IsAddable(equipment))
            {
                // 付けている装備を外す
                removed = inventory.RemoveItem(remove);

                if (!inventory.IsAddable(equipment))
                    throw new ArgumentException(nameof(remove));
            }
            
            var added = inventory.AddItem(equipment);
            var changeResult = player.ChangeBattleEquipment(equipment);

            if (removed != null && removed != changeResult.PrevEquipment)
                throw new InvalidOperationException("インベントリとBattleModeuleから取り除いたアイテムが一致しません");

            if (added != changeResult.CurrentEquipment)
                throw new InvalidOperationException("インベントリとBattleModuleから新たに装着したアイテムが一致しません");
        }
    }
}
