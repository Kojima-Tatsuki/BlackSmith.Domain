using BlackSmith.Domain.Character;
using System;
using System.Collections.Generic;

namespace BlackSmith.Domain.Item
{
    internal class CraftableItemService
    {
        // アイテムの作成にあたって、作成者を要求するようにしている
        // 今後、作成に制約を持たせる際には、ここのロジックにその条件を記載する可能性がある
        // クラフトレシピにも同じく、記載される可能性があり、その際にどちらが適切かを考える
        // 現段階では、レシピではなくこのクラスのロジックとして新たな制約は記載されることが望ましいと思う

        /// <summary>アイテムの作成を行う</summary>
        /// <remarks>素材アイテムは過不足無く与える必要がある</remarks>
        /// <param name="recipe">作成するアイテムのレシピ</param>
        /// <param name="materials">素材、過不足なく与える必要がある</param>
        /// <returns></returns>
        internal CraftingResult Craft(CraftingRecipe recipe, IReadOnlyList<ICraftMaterialItem> materials, CharacterID creator)
        {
            if (recipe == null) throw new ArgumentNullException("Not found CraftingRecipe. (BTXyFn8e)");
            if (materials == null) throw new ArgumentNullException("Not fount CraftMaterials. (fk3XICGo)");

            // 素材が揃っているなら必ず製作は成功する
            if (!recipe.IsCraftable(materials))
                return new CraftingResult(false, null, creator);

            return new CraftingResult(
                success: true,
                craftable: recipe.Craftable,
                creator: creator);
        }
    }

    // 作成の際に使用した素材や、作成日時などのデータが追加されることが想定できる
    public class CraftingResult
    {
        public bool Success { get; } // 作製が成功したか

        /// <summary>アイテムの作成に失敗した場合は、NULLとなる</summary>
        public ICraftableItem? Craftable { get; }

        // 作成の失敗、成功によらず、作成者は格納する
        public CharacterID Creator { get; }

        internal CraftingResult(bool success, ICraftableItem? craftable, CharacterID creator)
        {
            Success = success;
            Craftable = craftable;
            Creator = creator;
        }
    }
}
