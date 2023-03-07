using System.Collections;
using System.Collections.Generic;

namespace BlackSmith.Domain.Item
{
    /// <summary>
    /// Item�̐݌v���ꎞ�I�ɂ邽�߂̃e�X�g�N���X
    /// </summary>
    internal class PrototypeItem
    {
        /// <summary> �A�C�e���� </summary>
        public ItemName Name { get; private set; } = null!;
    }

    /// <summary> ����A�C�e�� </summary>
    public interface IConsumeItem
    {
        public int CoolingTime { get; }
    }

    public interface IHealableItem : IConsumeItem
    {
        /// <summary> �񕜗� </summary>
        public int HealValue { get; }
    }

    public interface IEffectableItem : IConsumeItem
    {
        public IReadOnlyCollection<ItemEffectType> ItemEffects { get; }

        public enum ItemEffectType 
        {
            Poison
        }
    }

    /// <summary> �Ȃ�ł����� </summary>
    internal class AllRecoverItem : PrototypeItem, IHealableItem, IEffectableItem
    {
        public int HealValue { get; private set; }

        public int CoolingTime { get; private set; }

        public IReadOnlyCollection<IEffectableItem.ItemEffectType> ItemEffects { get; }

        public AllRecoverItem(int healthValue, int coolingTime, IReadOnlyCollection<IEffectableItem.ItemEffectType> effectTypes)
        {
            HealValue = healthValue;
            CoolingTime = coolingTime;
            ItemEffects = effectTypes;
        }
    }
}
