using BlackSmith.Domain.PassiveEffect;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

#nullable enable

namespace BlackSmith.Domain.Character.Battle
{
    public record BattleStatusEffectModule
    {
        public IReadOnlyCollection<BattleStatusEffect> StatusEffects => statusEffects.Values.ToImmutableList();

        private readonly ImmutableDictionary<EffectID, BattleStatusEffect> statusEffects;

        [JsonConstructor]
        internal BattleStatusEffectModule(IReadOnlyCollection<BattleStatusEffect>? statusEffects = null)
        {
            this.statusEffects = (statusEffects ?? ImmutableList.Create<BattleStatusEffect>())
                .ToImmutableDictionary(x => x.Id);
        }

        internal BattleStatusEffectModule AddStatusEffect(BattleStatusEffect statusEffect)
        {
            // 追加して生成
            var dict = new Dictionary<EffectID, BattleStatusEffect>(statusEffects);

            if (dict.Keys.Contains(statusEffect.Id))
                throw new InvalidOperationException($"The effect aleady exists. id: {statusEffect.Id}. (RiaYLr6o)");

            dict.Add(statusEffect.Id, statusEffect);

            return new BattleStatusEffectModule(dict.Values.ToImmutableList());
        }

        internal BattleStatusEffectModule RemoveStatusEffect(BattleStatusEffect statusEffect)
        {
            var dict = new Dictionary<EffectID, BattleStatusEffect>(statusEffects);

            if (!dict.Keys.Contains(statusEffect.Id))
                throw new InvalidOperationException($"Does not exist the effect. id: {statusEffect.Id}. (NXL6wtjY)");

            dict.Remove(statusEffect.Id);

            return new BattleStatusEffectModule(dict.Values.ToImmutableList());
        }

        public virtual bool Equals(BattleStatusEffectModule? other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return statusEffects.SequenceEqual(other.statusEffects);
        }

        public override int GetHashCode()
        {
            return statusEffects.GetHashCode();
        }
    }
}
