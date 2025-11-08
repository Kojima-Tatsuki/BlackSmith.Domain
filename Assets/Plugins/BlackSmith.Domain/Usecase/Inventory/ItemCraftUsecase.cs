using BlackSmith.Domain.Inventory;
using BlackSmith.Domain.Item;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlackSmith.Usecase.Inventory
{
    /// <summary>
    /// <see cref="CraftCommand"/>を用いてアイテムを作成する機能を提供する.<br/>
    /// 作成にあたり、作成条件などはアイテムに関してのみ存在する.
    /// </summary>
    public class ItemCraftUsecase
    {
        /// <summary>アイテムの作成を行う</summary>
        /// <param name="command">作成のコマンド</param>
        /// <returns>作成し、追加されたアイテム</returns>
        public ICraftableItem Craft(CraftCommand command)
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
                .All(material => command.FromInventory.Contains(material)))
                //throw new ArgumentException("一部、素材がたりません");
                return false;

            if (!command.ToInventory.IsAddableItem(command.Recipe.Craftable))
                //throw new ArgumentException("作成したアイテムがインベントリに追加できません");
                return false;

            return true;
        }

        public class CraftCommand
        {
            public readonly IReadOnlyList<ICraftMaterialItem> Materials;
            public readonly CraftingRecipe Recipe;
            public readonly IInventoryService FromInventory, ToInventory;

            public CraftCommand(IReadOnlyList<ICraftMaterialItem> materials, CraftingRecipe recipe, IInventoryService fromInventory, IInventoryService toInventory)
            {
                Materials = materials ?? throw new ArgumentNullException(nameof(materials));
                Recipe = recipe ?? throw new ArgumentNullException(nameof(recipe));
                FromInventory = fromInventory ?? throw new ArgumentNullException(nameof(fromInventory));
                ToInventory = toInventory ?? throw new ArgumentNullException(nameof(toInventory));
            }
        }
    }
}
