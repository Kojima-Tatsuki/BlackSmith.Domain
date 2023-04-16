using System;
using System.Collections.Generic;
using System.Linq;
using BlackSmith.Domain.Item;
using BlackSmith.Domain.Inventory;

namespace BlackSmith.Usecase.Inventory
{
    public class ItemCraftUsecase
    {
        /// <summary>アイテムの作成を行う</summary>
        /// <param name="command">作成のコマンド</param>
        /// <returns>作成し、追加されたアイテム</returns>
        public ICraftable Craft(CraftCommand command)
        {
            if (command is null)
                throw new ArgumentNullException(nameof(command));

            if (!IsCraftable(command))
                throw new ArgumentException("アイテムの作成に失敗しました");

            command.Recipe.Materials
                .ToList()
                .ForEach(material => command.FromInventory.RemoveItem(material));
            command.ToInventory.AddItem(command.Recipe.Craftable);

            return command.Recipe.Craftable;
        }

        /// <summary>アイテムが作成可能かを返す</summary>
        /// <param name="command">作成のコマンド</param>
        /// <returns></returns>
        public bool IsCraftable(CraftCommand command)
        {
            if (!command.Recipe.Materials
                .All(material => command.FromInventory.IsContain(material)))
                //throw new ArgumentException("一部、素材がたりません");
                return false;

            if (!command.ToInventory.IsAddable(command.Recipe.Craftable))
                //throw new ArgumentException("作成したアイテムがインベントリに追加できません");
                return false;

            return true;
        }

        public class CraftCommand
        {
            public readonly IReadOnlyList<IMaterial> Materials;
            public readonly CraftingRecipe Recipe;
            public readonly IInventoryService FromInventory, ToInventory;

            public CraftCommand(IReadOnlyList<IMaterial> materials, CraftingRecipe recipe, IInventoryService fromInventory, IInventoryService toInventory)
            {
                Materials = materials ?? throw new ArgumentNullException(nameof(materials));
                Recipe = recipe ?? throw new ArgumentNullException(nameof(recipe));
                FromInventory = fromInventory ?? throw new ArgumentNullException(nameof(fromInventory));
                ToInventory = toInventory ?? throw new ArgumentNullException(nameof(toInventory));
            }
        }
    }
}
