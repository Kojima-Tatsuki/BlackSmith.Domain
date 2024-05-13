using System;

namespace BlackSmith.Domain.PassiveEffect
{
    // 直接的なリポジトリ管理はしない
    internal record BattleStatusEffect
    {
        internal EffectID Id { get; } // IDを重複刺せないのであれば、Idのみを使用したEqualsを実装すべき？

        internal BattleStatusEffectModel StatusModel { get; }

        internal BattleStatusEffect(EffectID id, BattleStatusEffectModel statusModel)
        {
            Id = id;
            StatusModel = statusModel;
        }
    }

    internal record EffectID
    {
        internal Guid Value { get; }

        internal EffectID()
        {
            Value = Guid.NewGuid();
        }
    }

    internal record BattleStatusEffectModel
    {
        public int MaxHealth { get; }
        public int Attack { get; }
        public int Defense { get; }
        public int MovementSpeed { get; }

        internal BattleStatusEffectModel(int maxHealth, int attack, int defense, int moveSpeed)
        {
            MaxHealth = maxHealth;
            Attack = attack;
            Defense = defense;
            MovementSpeed = moveSpeed;
        }
    }
}
