using System.Collections.Generic;
using System.Linq;
using BlackSmith.Domain.Item;
using R3;

namespace BlackSmith.Domain.Inventory
{
    public interface ICharacterInventoryEventPublisher<T> where T : IItem
    {
        public Observable<InventoryChangeCarrier<T>> OnChanged { get; }
    }

    /// <summary>
    /// インベントリ内のアイテムの変更情報を保持する
    /// </summary>
    /// <typeparam name="T">インベントリに格納されているアイテムの型</typeparam>
    public class InventoryChangeCarrier<T> where T : IItem
    {
        public IReadOnlyDictionary<T, int> Current { get; }

        public IReadOnlyDictionary<T, int> Prev { get; }

        public InventoryChangeCarrier(IReadOnlyDictionary<T, int> prev, IReadOnlyDictionary<T, int> current)
        {
            Current = current;
            Prev = prev;
        }

        public IReadOnlyList<KeyValuePair<T, int>> GetChangedItems()
        {
            return Current.Except(Prev).ToList();
        }
    }
}
