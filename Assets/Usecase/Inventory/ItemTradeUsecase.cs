using System;
using System.Collections.Generic;
using System.Linq;
using BlackSmith.Domain.Item;
using BlackSmith.Domain.Inventory;

namespace BlackSmith.Usecase.Inventory
{
    /// <summary>インベントリ間でアイテムの受け渡しを行う際に使用する</summary>
    public class ItemTradeUsecase
    {
        /// <summary>インベントリ間でアイテムを移動させる</summary>
        /// <typeparam name="T">アイテムクラスを継承したアイテム</typeparam>
        /// <param name="fromInventory">移動元のインベントリ</param>
        /// <param name="toInventory">移動先のインベントリ</param>
        /// <param name="item">移動させるアイテム</param>
        public void Transfer<T>(IInventoryService<T> fromInventory, IInventoryService<T> toInventory, T item) where T: Item
        {
            if (!fromInventory.IsContain(item))
                throw new ArgumentException("itemがfromInventoryに存在しません");

            if (!toInventory.IsAddable(item))
                throw new ArgumentException("itemは移動先のインベントリに追加できません");

            fromInventory.RemoveItem(item);
            toInventory.AddItem(item);
        }
    }
}
