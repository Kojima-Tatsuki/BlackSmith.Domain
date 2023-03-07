﻿
using System;
using System.Collections.Generic;
using System.Linq;
using BlackSmith.Domain.Item;
using BlackSmith.Domain.Item.Equipment;

namespace BlackSmith.Domain.Inventory
{
    using Item;

    /// <summary>装備中のアイテムを格納するインベントリ</summary>
    public class EquipmentInventory : IOneByInventoryService<EquippableItem>
    {
        private InventoryID ID { get; }

        private readonly Dictionary<EquipmentType, EquippableItem> Equipments;

        public EquipmentInventory(InventoryID id)
        {
            ID = id ?? throw new ArgumentNullException(nameof(id));

            Equipments = new Dictionary<EquipmentType, EquippableItem>();
        }

        public EquippableItem AddItem(EquippableItem item)
        {
            if (IsOccupiedType(item.EquipType))
                throw new ArgumentException("既に別のアイテムが装備されています");

            Equipments.Add(item.EquipType, item);

            return item;
        }

        public EquippableItem RemoveItem(EquippableItem item)
        {
            if (!IsContain(item))
                throw new ArgumentException("指定のアイテムは装備されていません");

            Equipments.Remove(item.EquipType);

            return item;
        }

        public int GetContainItemCount(EquippableItem item)
        {
            if (!IsOccupiedType(item.EquipType))
                return 0;

            if (item.Equals(Equipments[item.EquipType]))
                return 1;

            return 0;
        }

        public IReadOnlyCollection<EquippableItem> GetContainItems()
        {
            return Equipments.Values;
        }

        public IReadOnlyDictionary<EquippableItem, int> GetInventory()
        {
            return new Dictionary<EquippableItem, int>(
                Equipments.Select(typeAndItem => new KeyValuePair<EquippableItem, int>(typeAndItem.Value, 1)));
        }

        public bool IsContain(EquippableItem item)
        {
            if (!IsOccupiedType(item.EquipType))
                return false;

            if (item.Equals(Equipments[item.EquipType]))
                return true;

            return false;
        }

        public bool IsAddable(EquippableItem item)
        {
            if (item is null)
                return false;

            if (IsOccupiedType(item.EquipType))
                return false;

            return true;
        }

        /// <summary>
        /// 指定位置の装備アイテムが装備されているかを返す
        /// </summary>
        /// <param name="type">装備位置</param>
        /// <returns>装備されているなら真を返す</returns>
        private bool IsOccupiedType(EquipmentType type) => Equipments.ContainsKey(type);
    }
}
