using System;
using System.Collections.Generic;
using System.Linq;
using BlackSmith.Domain.Player;

namespace BlackSmith.Domain.Item
{
    /// <summary>素材となることができるアイテム</summary>
    public interface IMaterial : IItem
    {

    }

    /// <summary>素材アイテムを使用して作成ができるアイテム</summary>
    public interface ICraftable : IItem
    {
        PlayerID CreatedBy { get; } // 制作者ID
        IReadOnlyCollection<IMaterial> GetRequireMaterials();
    }

    /// <summary>レシピ</summary>
    public class CraftingRecipe
    {
        /// <summary>レシピを使用して完成するアイテム</summary>
        public readonly ICraftable Craftable;
        // レシピには順序の概念が存在する
        public readonly IReadOnlyList<IMaterial> Materials;

        public CraftingRecipe(ICraftable craftable, IReadOnlyList<IMaterial> materials)
        {
            Craftable = craftable;
            Materials = materials;
        }

        // 素材がそろっているかどうかを考える段階では、順序は考えない
        /// <summary>素材の必要量を満たしているかを返す</summary>
        /// <remarks>引数は必要量以上に素材が存在してもよい、満たない場合はFalseが返る</remarks>
        /// <param name="materials">判定する素材</param>
        /// <returns>必要量を満たすか</returns>
        public bool IsCraftable(IReadOnlyCollection<IMaterial> materials)
        {
            return Materials.All(item => materials.Contains(item));
        }
    }
}
