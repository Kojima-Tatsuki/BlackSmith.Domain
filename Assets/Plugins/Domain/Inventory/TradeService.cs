using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackSmith.Domain.Inventory
{
    using Item;
    using Currency;

    public class TradeItem
    {
        public IInventoryService<Item> Items { get; }
        public Currency Money { get; }

        public TradeItem(IInventoryService<Item> items, Currency money)
        {
            Items = items;
            Money = money;
        }
    }
}
