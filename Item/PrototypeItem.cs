using System.Collections;
using System.Collections.Generic;

namespace BlackSmith.Domain.Item
{
    /// <summary>
    /// Itemの設計を一時的にるためのテストクラス
    /// </summary>
    internal class PrototypeItem
    {
        /// <summary> アイテム名 </summary>
        public ItemName Name { get; private set; } = null!;
    }

    /// <summary> 消費アイテム </summary>
    public interface IConsumeItem
    {
        public int CoolingTime { get; }
    }

    public interface IHealableItem : IConsumeItem
    {
        /// <summary> 回復量 </summary>
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

    /// <summary> なんでも治し </summary>
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
