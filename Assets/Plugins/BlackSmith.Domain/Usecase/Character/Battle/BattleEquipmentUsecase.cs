using BlackSmith.Domain.Character.Player;
using BlackSmith.Domain.CharacterObject;
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

        public void ChengeEquipment(PlayerID playerId, InventoryID inventoryId, EquippableItem equipment, EquippableItem remove)
        {
            var player = PlayerRepository.FindByID(playerId) ?? throw new InvalidOperationException(nameof(playerId));
            var inventoryService = InventoryRepository.FindByID(inventoryId) ?? throw new InvalidOperationException(nameof(inventoryId));

            var inventory = (inventoryService as EquipmentInventory) ?? throw new ArgumentException(nameof(inventoryId));

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

        public void RemoveEquipment(PlayerID playerId, InventoryID inventoryId, EquippableItem remove)
        {
            var player = PlayerRepository.FindByID(playerId) ?? throw new InvalidOperationException(nameof(playerId));
            var inventoryService = InventoryRepository.FindByID(inventoryId) ?? throw new InvalidOperationException(nameof(inventoryId));

            var inventory = (inventoryService as EquipmentInventory) ?? throw new ArgumentException(nameof(inventoryId));

            var removed = inventory.RemoveItem(remove);
            var changeResult = player.RemoveBattleEquipment(remove.EquipType);

            if (removed != null && removed != changeResult.PrevEquipment)
                throw new InvalidOperationException("インベントリとBattleModeuleから取り除いたアイテムが一致しません");
        }
    }
}
