using System;
using System.Collections.Generic;

#nullable enable

namespace BlackSmith.Domain.Item
{
    public interface IItem
    {
        string Name { get; }
    }

    /// <summary>このItemに関するデータの変更等を行う際の窓口として使用する</summary>
    public record Item : IItem
    {
        public string Name => itemName.Value;
        private protected readonly ItemName itemName;

        internal Item(string itemName)
        {
            if (itemName is null) throw new ArgumentNullException(nameof(itemName));

            this.itemName = new ItemName(itemName);
        }
    }

    /// <summary>アイテムを一意に定めることのできる識別子</summary>
    public class ItemID : BasicID
    {
        protected override string Prefix => "Item-";
    }

    /// <summary>
    /// アイテム名
    /// </summary>
    internal record ItemName
    {
        public string Value { get; }

        internal ItemName(string name)
        {
            if (name is null) throw new ArgumentNullException(nameof(name));
            if (name.Length == 0) throw new ArgumentOutOfRangeException("nameは1文字以上です。");

            Value = name;
        }
    }

    internal enum ItemType
    {
        None = 0x0,  // なし
        Weapon = 0x1,  // 武器
        Armor = 0x2,  // 防具
        Consum = 0x4,  // 消費アイテム
    }


    /// <summary>
    /// アイテムの比較を行う
    /// </summary>
    /// <remarks>Dictionaryのコンストラクタの引数として渡すと、処理が早くなる</remarks>
    class ItemComparer : IEqualityComparer<Item>
    {
        public bool Equals(Item? x, Item? y)
        {
            if (x == null || y == null)
                return false;
            return x.Equals(y);
        }

        public int GetHashCode(Item obj) => obj.GetHashCode();
    }
}