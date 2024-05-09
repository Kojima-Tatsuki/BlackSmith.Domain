using BlackSmith.Domain.PassiveEffect;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace BlackSmith.Domain.Character.Battle
{
    internal class BlattleStatusEffectModule
    {
        internal IReadOnlyCollection<BattleStatusEffect> StatusEffects => StatusEffectDictionary.Values.ToList();
        private IReadOnlyDictionary<EffectID, BattleStatusEffect> StatusEffectDictionary { get; }

        internal BlattleStatusEffectModule(IReadOnlyDictionary<EffectID, BattleStatusEffect>? statusEffects = null)
        {
            statusEffects ??= new Dictionary<EffectID, BattleStatusEffect>();
            StatusEffectDictionary = new Dictionary<EffectID, BattleStatusEffect>(statusEffects);
        }

        internal BlattleStatusEffectModule AddStatusEffect(BattleStatusEffect statusEffect)
        {
            // 追加して生成
            var dict = new Dictionary<EffectID, BattleStatusEffect>(StatusEffectDictionary);

            if (dict.Keys.Contains(statusEffect.Id))
                throw new InvalidOperationException($"The effect aleady exists. id: {statusEffect.Id}. (RiaYLr6o)");

            dict.Add(statusEffect.Id, statusEffect);

            return new BlattleStatusEffectModule(dict);
        }

        internal BlattleStatusEffectModule RemoveStatusEffect(BattleStatusEffect statusEffect)
        {
            var dict = new Dictionary<EffectID, BattleStatusEffect>(StatusEffectDictionary);

            if (!dict.Keys.Contains(statusEffect.Id))
                throw new InvalidOperationException($"Does not exist the effect. id: {statusEffect.Id}. (NXL6wtjY)");

            dict.Remove(statusEffect.Id);

            return new BlattleStatusEffectModule(dict);
        }
    }
}
