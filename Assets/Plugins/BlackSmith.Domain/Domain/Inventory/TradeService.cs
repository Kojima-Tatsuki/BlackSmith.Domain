namespace BlackSmith.Domain.Inventory
{
    using Currency;
    using Item;

    internal class TradeItem
    {
        internal IInventoryService<Item> Items { get; }
        internal Currency Money { get; }

        internal TradeItem(IInventoryService<Item> items, Currency money)
        {
            Items = items;
            Money = money;
        }
    }
}
