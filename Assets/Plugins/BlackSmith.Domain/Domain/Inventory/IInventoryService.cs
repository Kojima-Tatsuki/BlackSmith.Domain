using System.Collections.Generic;

#nullable enable

namespace BlackSmith.Domain.Inventory
{
    using Currency;
    using Item;

    public interface IInventoryService : IInventoryService<IItem>, IInventoryStateViewable<IItem> { }

    public interface IInventoryStateViewable : IInventoryStateViewable<IItem> { }

    /// <summary>
    /// インベントリの操作をする際に使うInterface
    /// </summary>
    /// <typeparam name="T">インベントリに格納するアイテムの型</typeparam>
    public interface IInventoryService<T> : IInventoryStateViewable<T> where T : IItem
    {
        /// <summary>
        /// アイテムを追加する
        /// </summary>
        /// <param name="item">追加するアイテム</param>
        /// <param name="count">追加する個数</param>
        /// <returns>追加したアイテム</returns>
        T AddItem(T item, int count = 1!);

        /// <summary>
        /// アイテムを取り除く
        /// </summary>
        /// <param name="item">取り除くアイテム</param>
        /// <param name="count">取り除く個数</param>
        /// <returns>取り除いたアイテム</returns>
        T RemoveItem(T item, int count = 1!);

        /// <summary>
        /// アイテムがインベントリ内にあるかを返す
        /// </summary>
        /// <param name="item">探索するアイテム</param>
        /// <returns>存在すれば真を返す</returns>
        bool Contains(T item);

        bool IsAddable(T item, int count = 1!);
    }

    /// <summary>1つずつしか操作が行えないインベントリ</summary>
    public interface IOneByInventoryService<T> : IInventoryStateViewable<T> where T : IItem
    {
        /// <summary>
        /// アイテムを追加する
        /// </summary>
        /// <param name="item">追加するアイテム</param>
        /// <returns>追加したアイテム</returns>
        T AddItem(T item);

        /// <summary>
        /// アイテムを取り除く
        /// </summary>
        /// <param name="item">取り除くアイテム</param>
        /// <returns>取り除いたアイテム</returns>
        T RemoveItem(T item);

        /// <summary>
        /// アイテムがインベントリ内にあるかを返す
        /// </summary>
        /// <param name="item">探索するアイテム</param>
        /// <returns>存在すれば真を返す</returns>
        bool Contains(T item);

        bool IsAddable(T item);
    }

    /// <summary>
    /// インベントリの情報を表示可能
    /// </summary>
    /// <typeparam name="T">インベントリに格納するアイテムの型</typeparam>
    public interface IInventoryStateViewable<T> where T : IItem
    {
        /// <summary>
        /// 格納されているすべてのアイテムとその個数を返す
        /// </summary>
        /// <returns>アイテムとその個数の辞書型</returns>
        IReadOnlyDictionary<T, int> GetInventory();

        /// <summary>
        /// 格納されているアイテムをすべて返す
        /// </summary>
        /// <returns>格納されているアイテム</returns>
        IReadOnlyCollection<T> GetContainItems();

        /// <summary>
        /// 対象のアイテムの所持数を返す
        /// </summary>
        /// <param name="item">対象のアイテム</param>
        /// <returns>所持数</returns>
        int GetContainItemCount(T item);
    }

    /// <summary>
    /// 金銭を扱う
    /// </summary>
    public interface IWallet
    {
        public void AdditionMoney(Currency money);

        public void SubtractMoney(Currency money);

        /// <summary>
        /// 所持金を返す
        /// </summary>
        /// <returns>所持金</returns>
        IReadOnlyCollection<Currency> GetMoney();

        Currency GetMoney(CurrencyType type);

        bool ContainsType(CurrencyType type);
    }
}