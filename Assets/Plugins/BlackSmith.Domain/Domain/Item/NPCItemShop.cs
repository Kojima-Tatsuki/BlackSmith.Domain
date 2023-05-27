using System;
using System.Collections.Generic;
using System.Linq;

namespace BlackSmith.Domain.Item
{
    using Currency;

    /*
    ショップの種類
    - 市場
    - 個人店
    - NPCショップ

    扱うお金、アイテムの保管場所
     市場 -> アイテム -> 専用のリポジトリ
             お金     -> 自動生成??
     個人店 -> 倉庫又はPlayerのインベントリ
     NPCショップ -> 定価 -> 保管場所なし、その場で生成

    外部が出来ること
     陳列済みアイテムの閲覧
     販売中のアイテムの購入
     
    */

    /// <summary>
    /// アイテムの買取が可能
    /// </summary>
    public interface IItemPurchasable
    {
        /// <summary>
        /// アイテムを売る
        /// </summary>
        /// <param name="item">売るアイテム</param>
        /// <returns>得るお金</returns>
        Currency SellItem(Item item);
    }

    /// <summary>
    /// アイテムの出品が可能
    /// </summary>
    public interface IItemExhibitable
    {
        /// <summary>
        /// 出品中のアイテムを購入する
        /// </summary>
        /// <param name="item">購入するアイテム</param>
        /// <param name="currency">購入するアイテムの金額</param>
        /// <returns>購入したアイテム</returns>
        Item BuyItem(Item item, Currency currency);

        /// <summary>販売中のアイテムをすべて返す</summary>
        /// <remarks>金額はすべて単価</remarks>
        IReadOnlyCollection<Item> GetItemsOnSale();

        /// <summary>
        /// アイテムの販売単価を返す
        /// </summary>
        /// <param name="item">単価を調べるアイテム</param>
        /// <param name="type">金額の通貨規格</param>
        /// <returns>販売単価</returns>
        Currency GetUnitPriceOfItem(Item item, CurrencyType type = CurrencyType.Sakura);

        bool IsExist(Item item);
    }

    /// <summary>
    /// NPCのアイテム販売店
    /// </summary>
    /// <remarks>在庫は無限大、価格は一定</remarks>
    public class NPCItemShop : IItemExhibitable
    {
        /// <summary>
        /// 販売中のアイテムのリスト
        /// </summary>
        private readonly Dictionary<Item, Currency> exhibitItems;

        public NPCItemShop(IReadOnlyCollection<ItemAndPrice> list)
        {
            exhibitItems = new Dictionary<Item, Currency>(new ItemComparer());

            foreach (var item in list)
            {
                exhibitItems.Add(item.Item, item.Currency);
            }
        }

        public Item BuyItem(Item item, int count, Currency currency)
        {
            if (!IsExist(item))
                throw new ArgumentNullException(
                    $"入力したアイテムは出品されていない," +
                    $"入力したアイテム:{item}");

            var unitPrice = GetUnitPriceOfItem(item, currency.Type);

            if (unitPrice.Value * count != currency.Value)
                throw new ArgumentException(
                    $"アイテムの金額と入力した金額が一致していない, " +
                    $"出品表示額:{exhibitItems[item]}, " +
                    $"入力金額:{currency}");

            return item;
        }

        public Item BuyItem(Item item, Currency currency)
        {
            if (!IsExist(item))
                throw new ArgumentNullException(
                    $"入力したアイテムは出品されていない," +
                    $"入力したアイテム:{item}");

            if (GetUnitPriceOfItem(item, currency.Type) != currency)
                throw new ArgumentException(
                    $"アイテムの金額と入力した金額が一致していない, " +
                    $"出品表示額:{exhibitItems[item]}, " +
                    $"入力金額:{currency}");

            return item;
        }

        public IReadOnlyCollection<Item> GetItemsOnSale() => exhibitItems.Keys;

        public Currency GetUnitPriceOfItem(Item item, CurrencyType type = CurrencyType.Sakura)
        {
            if (!IsExist(item))
                throw new ArgumentNullException(
                    $"入力したアイテムは出品されていない," +
                    $"入力したアイテム:{item}");

            return exhibitItems[item].Exchange(type);
        }

        public bool IsExist(Item item) => exhibitItems.ContainsKey(item);

        public class ItemAndPrice
        {
            public Item Item { get; }

            public Currency Currency { get; }

            public ItemAndPrice(Item item, Currency currency)
            {
                Item = item ?? throw new ArgumentNullException(nameof(item));

                Currency = currency ?? throw new ArgumentNullException(nameof(currency));
            }
        }
    }
}
