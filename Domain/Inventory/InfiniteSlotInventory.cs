using BlackSmith.Domain.Item;
using System;
using System.Linq;
using System.Collections.Generic;

#nullable enable

namespace BlackSmith.Domain.Inventory
{
    using Item;

    /// <summary>
    /// ItemSlotを無限個格納するInventory
    /// </summary>
    internal class InfiniteSlotInventory : IInventoryService
    {
        private InventoryID ID { get; }

        private Dictionary<IItem, ItemSlot> ItemSlots { get; }

        private int InitializeSlotCapacity { get; } = 32;

        internal InfiniteSlotInventory(InventoryID id)
        {
            ID = id ?? throw new ArgumentNullException(nameof(id));

            ItemSlots = new Dictionary<IItem, ItemSlot>(InitializeSlotCapacity);
        }

        public IItem AddItem(IItem item, int count = 1!)
        {
            if (!IsAddable(item, count))
                throw new ArgumentException($"そのアイテムはこのインベントリに追加できません, item: {item}");

            var num = new ItemCountNumber(count);

            if (IsContain(item))
            {
                ItemSlots[item] = ItemSlots[item].AddItem(item, num);
            }
            else // スロットを新たに作成
            {
                var slot = new ItemSlot(item, num);
                ItemSlots.Add(item, slot);
            }

            return item;
        }

        public IItem RemoveItem(IItem item, int count = 1!)
        {
            if (item is null)
                throw new ArgumentNullException(nameof(item));

            var num = new ItemCountNumber(count);

            if (!IsContain(item))
                throw new NullReferenceException("このインベントリにはそのようなアイテムは存在しません");

            if (num > ItemSlots[item].Count)
                throw new ArgumentOutOfRangeException("所持数以上のアイテムを削除しようとしました");

            ItemSlots[item] = ItemSlots[item].RemoveItem(item, num);

            return item;
        }

        public bool IsContain(IItem item)
        {
            return ItemSlots.ContainsKey(item);
        }

        public bool IsAddable(IItem item, int count = 1)
        {
            if (item is null)
                return false;

            return true;
        }

        /// <summary>
        /// 探索アイテムが含まれているアイテムの個数を返す
        /// </summary>
        /// <param name="item">探索するアイテム</param>
        /// <returns></returns>
        public ItemCountNumber ContainingItemCount(IItem item)
        {
            if (item is null)
                throw new ArgumentNullException(nameof(item));

            if (!IsContain(item))
                return new ItemCountNumber(0);

            return ItemSlots[item].Count;
        }

        public IReadOnlyDictionary<IItem, int> GetInventory()
        {
            return new Dictionary<IItem, int>(ItemSlots
                .Select(slots => new KeyValuePair<IItem, int>(slots.Key, slots.Value.Count.Value)));
        }

        IReadOnlyCollection<IItem> IInventoryStateViewable<IItem>.GetContainItems() => ItemSlots.Keys;

        int IInventoryStateViewable<IItem>.GetContainItemCount(IItem item) => ContainingItemCount(item).Value;
    }

    public class InventoryID : BasicID
    {
        internal InventoryID(Guid id) : base(id)
        {
        }
    }

    public class InventoryCapacity
    {
        public int Value { get; }

        public InventoryCapacity(int value)
        {
            if (value < 0) throw new ArgumentOutOfRangeException(nameof(value));
            this.Value = value;
        }
    }
}