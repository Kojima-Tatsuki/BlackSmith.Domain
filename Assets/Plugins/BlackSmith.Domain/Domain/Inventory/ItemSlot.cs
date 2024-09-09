using BlackSmith.Domain.Item;
using System;

#nullable enable

namespace BlackSmith.Domain.Inventory
{
    using T = IItem;

    // ItemSlot は ValueObject
    /// <summary>
    /// 主にアイテムの所有数を管理する
    /// InventoryとItemとの仲介役
    /// </summary>
    internal class ItemSlot
    {
        internal T Item { get; }
        internal ItemCountNumber Count { get; }

        internal ItemSlot(T item, ItemCountNumber count)
        {
            Item = item;
            Count = count;
        }

        internal ItemSlot AddItem(T item, ItemCountNumber count = null!)
        {
            count ??= new ItemCountNumber(1);

            if (item is null)
                throw new ArgumentNullException(nameof(item));

            if (!IsContaining(item))
                throw new ArgumentException("itemとitemが一致しません。");

            return new ItemSlot(Item, Count.Add(count));
        }

        internal ItemSlot RemoveItem(T item, ItemCountNumber count = null!)
        {
            count ??= new ItemCountNumber(1);

            if (item is null)
                throw new ArgumentNullException(nameof(item));

            if (!IsContaining(item))
                throw new ArgumentException("itemとitemが一致しません。");

            return new ItemSlot(item, Count.Subtract(count));
        }

        /// <summary>
        /// このスロットにアイテムが格納されているかを返す
        /// </summary>
        /// <param name="item">調査するアイテム</param>
        /// <returns>アイテムが含まれているか</returns>
        internal bool IsContaining(T item)
        {
            return Item.Equals(item);
        }
    }

    /// <summary>
    /// アイテムを数えるときに扱う
    /// </summary>
    internal class ItemCountNumber : IEquatable<ItemCountNumber>
    {
        internal int Value { get; }

        internal ItemCountNumber(int value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value));

            Value = value;
        }

        internal ItemCountNumber Add(ItemCountNumber count)
        {
            if (count is null) throw new ArgumentNullException(nameof(count));

            return new ItemCountNumber(Value + count.Value);
        }

        internal ItemCountNumber Subtract(ItemCountNumber count)
        {
            if (count is null) throw new ArgumentNullException(nameof(count));

            var result = Value - count.Value;
            if (result < 0)
                throw new Exception("計算後の値が規定値を超えました。");

            return new ItemCountNumber(Value - count.Value);
        }

        public bool Equals(ItemCountNumber? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return Value == other.Value;
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (GetType() != obj.GetType()) return false;
            return Equals((ItemCountNumber)obj);
        }

        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(ItemCountNumber x, ItemCountNumber y)
            => x.Value == y.Value;

        public static bool operator !=(ItemCountNumber x, ItemCountNumber y)
            => x.Value != y.Value;

        public static bool operator <(ItemCountNumber x, ItemCountNumber y)
            => x.Value < y.Value;

        public static bool operator <=(ItemCountNumber x, ItemCountNumber y)
            => x.Value <= y.Value;

        public static bool operator >(ItemCountNumber x, ItemCountNumber y)
            => x.Value > y.Value;

        public static bool operator >=(ItemCountNumber x, ItemCountNumber y)
            => x.Value >= y.Value;

        public override string ToString() => Value.ToString();
    }

    internal class ItemHoldableCapacity
    {
        internal ItemCountNumber Value { get; }

        private const int MAXIMUM_CAPACITY = 64;

        internal ItemHoldableCapacity(ItemCountNumber capacity)
        {
            if (capacity is null)
                throw new ArgumentNullException(nameof(capacity));
            if (capacity.Value <= 0 || MAXIMUM_CAPACITY < capacity.Value) // cap: 1 ~ 64
                throw new ArgumentOutOfRangeException(nameof(capacity));

            Value = capacity;
        }
    }
}