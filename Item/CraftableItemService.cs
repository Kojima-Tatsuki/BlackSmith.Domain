using BlackSmith.Domain.Player;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace BlackSmith.Domain.Item
{
    internal class CraftableItemService
    {
        /// <summary>アイテムの作成を行う</summary>
        /// <remarks>素材アイテムは過不足無く与える必要がある</remarks>
        /// <param name="recipe">作成するアイテムのレシピ</param>
        /// <param name="materials">素材、過不足なく与える必要がある</param>
        /// <returns></returns>
        public CraftingResult Craft(CraftingRecipe recipe, IReadOnlyList<IMaterial> materials, PlayerID creator)
        {
            if (recipe == null)  throw new ArgumentNullException(nameof(recipe));
            if (materials == null) throw new ArgumentNullException(nameof(materials));

            // 素材が揃っているなら必ず製作は成功する
            if (!recipe.IsCraftable(materials))
                return new CraftingResult(false, null, null);

            return new CraftingResult(
                success: true,
                craftable: recipe.Craftable,
                creator: creator);
        }
    }

    public class CraftingResult
    {
        public bool Success { get; } // 作製が成功したか

        public ICraftable? Craftable { get; }

        public PlayerID? Creator { get; }

        public CraftingResult(bool success, ICraftable? craftable, PlayerID? creator)
        {
            Success = success;
            Craftable = craftable;
            Creator = creator;
        }
    }
}
