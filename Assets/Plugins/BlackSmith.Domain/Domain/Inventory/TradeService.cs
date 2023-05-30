using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackSmith.Domain.Inventory
{
    using Item;
    using Currency;

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
