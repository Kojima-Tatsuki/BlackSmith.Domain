using BlackSmith.Domain.Item;
using System.Collections;
using System.Collections.Generic;

namespace BlackSmith.Domain.Inventory 
{
    using Item;

    internal class UnCapacityItemSlot
    {
        public Item Item { get; private set; }

        public UnCapacityItemSlot(Item item)
        {
            Item = item;
        }
    }
}
