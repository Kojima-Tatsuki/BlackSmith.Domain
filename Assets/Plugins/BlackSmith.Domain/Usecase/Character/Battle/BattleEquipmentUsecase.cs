using BlackSmith.Domain.Character;
using BlackSmith.Domain.Inventory;
using BlackSmith.Domain.Item.Equipment;
using BlackSmith.Usecase.Interface;
using System;

#nullable enable

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

        public void ChengeEquipment(CharacterID playerId, InventoryID inventoryId, EquippableItem equipment, EquippableItem remove)
        {
            var player = PlayerRepository.FindByID(playerId) ?? throw new InvalidOperationException($"Player not found. playerId: {playerId}. (h2J7h7Ge)");
            var inventoryService = InventoryRepository.FindByID(inventoryId) ?? throw new InvalidOperationException($"InventoryService not found. inventoryId: {inventoryId}. (g098fssI)");

            var inventory = (inventoryService as IOneByInventoryService<EquippableItem>) ?? throw new InvalidCastException($"Id does not fill the requirement. inventoryId: {inventoryId}. (Hb3rQlG9)");

            EquippableItem? removed = null;

            if (!inventory.IsAddable(equipment))
            {
                // 付けている装備を外す
                removed = inventory.RemoveItem(remove);

                if (!inventory.IsAddable(equipment))
                    throw new InvalidOperationException($"Failed to attach the item. item: {equipment}. (VooMSeL5)");
            }

            var added = inventory.AddItem(equipment);
            var changeResult = player.ChangeBattleEquipment(equipment);

            if (removed != null && removed != changeResult.PrevEquipment)
                throw new InvalidOperationException("インベントリとBattleModeuleから取り除いたアイテムが一致しません. (jbhcw6XJ)");

            if (added != changeResult.CurrentEquipment)
                throw new InvalidOperationException("インベントリとBattleModuleから新たに装着したアイテムが一致しません. (MCqvxN67)");
        }

        public void RemoveEquipment(CharacterID playerId, InventoryID inventoryId, EquippableItem remove)
        {
            var player = PlayerRepository.FindByID(playerId) ?? throw new InvalidOperationException($"Player not found. playerId: {playerId}. (VrpFXAQ9)");
            var inventoryService = InventoryRepository.FindByID(inventoryId) ?? throw new InvalidOperationException($"InventoryService not found. inventoryId: {inventoryId}. (DjmTPN5D)");

            var inventory = (inventoryService as IOneByInventoryService<EquippableItem>) ?? throw new InvalidCastException($"Id does not fill the requirement. inventoryId: {inventoryId}. (JbmZeDf5)");

            var removed = inventory.RemoveItem(remove);
            var changeResult = player.RemoveBattleEquipment(remove.EquipType);

            if (removed != null && removed != changeResult.PrevEquipment)
                throw new InvalidOperationException("インベントリとBattleModeuleから取り除いたアイテムが一致しません. (OUy603NV)");
        }
    }
}
