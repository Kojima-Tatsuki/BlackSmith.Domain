using System;
using System.Collections.Generic;
using System.Linq;
using BlackSmith.Domain.Character;
using BlackSmith.Domain.Character.Player;

namespace BlackSmith.Domain.Item
{
    // Interfaceで定義しているものは、Enumで実装を置き換えることを考慮し続ける

    /// <summary>素材となることができるアイテム</summary>
    public interface ICraftMaterialItem : IItem
    {

    }

    /// <summary>素材アイテムを使用して作成ができるアイテム</summary>
    public interface ICraftableItem : IItem
    {
        CharacterID CreatedBy { get; } // 制作者ID
        IReadOnlyCollection<ICraftMaterialItem> GetRequireMaterials();
    }

    /// <summary>レシピ</summary>
    public class CraftingRecipe
    {
        /// <summary>レシピを使用して完成するアイテム</summary>
        public readonly ICraftableItem Craftable;
        // レシピには順序の概念が存在する
        public readonly IReadOnlyList<ICraftMaterialItem> Materials;

        // レシピオブジェクトの作成は、このライブラリに限る
        internal CraftingRecipe(ICraftableItem craftable, IReadOnlyList<ICraftMaterialItem> materials)
        {
            Craftable = craftable;
            Materials = materials;
        }

        // 素材がそろっているかどうかを考える段階では、順序は考えない
        /// <summary>素材の必要量を満たしているかを返す</summary>
        /// <remarks>引数は必要量以上に素材が存在してもよい、満たない場合はFalseが返る</remarks>
        /// <param name="materials">判定する素材</param>
        /// <returns>必要量を満たすか</returns>
        public bool IsCraftable(IReadOnlyCollection<ICraftMaterialItem> materials)
        {
            return Materials.All(item => materials.Contains(item));
        }
    }
}
