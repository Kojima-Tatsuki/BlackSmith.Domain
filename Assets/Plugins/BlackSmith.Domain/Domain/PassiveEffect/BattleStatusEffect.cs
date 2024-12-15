using Newtonsoft.Json;
using System;

namespace BlackSmith.Domain.PassiveEffect
{
    // 直接的なリポジトリ管理はしない
    public record BattleStatusEffect
    {
        public EffectID Id { get; } // IDを重複させないのであれば、Idのみを使用したEqualsを実装すべき？

        public BattleStatusEffectModel StatusModel { get; }

        [JsonConstructor]
        internal BattleStatusEffect(EffectID id, BattleStatusEffectModel statusModel)
        {
            Id = id;
            StatusModel = statusModel;
        }
    }

    public record EffectID
    {
        public Guid Value { get; }

        [JsonConstructor]
        internal EffectID(Guid? value = null)
        {
            Value = value ?? Guid.NewGuid();
        }
    }

    public record BattleStatusEffectModel
    {
        public int MaxHealth { get; }
        public int Attack { get; }
        public int Defense { get; }
        public int MovementSpeed { get; }

        [JsonConstructor]
        internal BattleStatusEffectModel(int maxHealth, int attack, int defense, int movementSpeed)
        {
            MaxHealth = maxHealth;
            Attack = attack;
            Defense = defense;
            MovementSpeed = movementSpeed;
        }
    }
}
