using System;
using System.Collections.Generic;
using System.Linq;
using BlackSmith.Domain.Character;
using BlackSmith.Domain.Character.Player;

namespace BlackSmith.Domain.Item
{
    // Interface�Œ�`���Ă�����̂́AEnum�Ŏ�����u�������邱�Ƃ��l����������

    /// <summary>�f�ނƂȂ邱�Ƃ��ł���A�C�e��</summary>
    public interface ICraftMaterialItem : IItem
    {

    }

    /// <summary>�f�ރA�C�e�����g�p���č쐬���ł���A�C�e��</summary>
    public interface ICraftableItem : IItem
    {
        CharacterID CreatedBy { get; } // �����ID
        IReadOnlyCollection<ICraftMaterialItem> GetRequireMaterials();
    }

    /// <summary>���V�s</summary>
    public class CraftingRecipe
    {
        /// <summary>���V�s���g�p���Ċ�������A�C�e��</summary>
        public readonly ICraftableItem Craftable;
        // ���V�s�ɂ͏����̊T�O�����݂���
        public readonly IReadOnlyList<ICraftMaterialItem> Materials;

        // ���V�s�I�u�W�F�N�g�̍쐬�́A���̃��C�u�����Ɍ���
        internal CraftingRecipe(ICraftableItem craftable, IReadOnlyList<ICraftMaterialItem> materials)
        {
            Craftable = craftable;
            Materials = materials;
        }

        // �f�ނ�������Ă��邩�ǂ������l����i�K�ł́A�����͍l���Ȃ�
        /// <summary>�f�ނ̕K�v�ʂ𖞂����Ă��邩��Ԃ�</summary>
        /// <remarks>�����͕K�v�ʈȏ�ɑf�ނ����݂��Ă��悢�A�����Ȃ��ꍇ��False���Ԃ�</remarks>
        /// <param name="materials">���肷��f��</param>
        /// <returns>�K�v�ʂ𖞂�����</returns>
        public bool IsCraftable(IReadOnlyCollection<ICraftMaterialItem> materials)
        {
            return Materials.All(item => materials.Contains(item));
        }
    }
}
