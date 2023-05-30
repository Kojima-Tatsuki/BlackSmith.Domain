using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackSmith.Domain.PassiveEffect
{
    // 直接的なリポジトリ管理はしない
    internal class BattleStatusEffect
    {
        internal EffectID Id { get; }

        internal BattleStatusEffectModel StatusModel { get; }

        internal BattleStatusEffect(EffectID id, BattleStatusEffectModel statusModel)
        {
            Id = id;
            StatusModel = statusModel;
        }
    }

    internal class EffectID
    {
        internal Guid Value { get; }

        internal EffectID()
        {
            Value = Guid.NewGuid();
        }
    }

    internal class BattleStatusEffectModel
    {
        public int MaxHealth { get; }
        public int Attack { get; }
        public int Defense { get; }
        public int MovementSpeed { get; }

        internal BattleStatusEffectModel(int maxHealth,  int attack, int defense, int moveSpeed)
        {
            MaxHealth = maxHealth;
            Attack = attack;
            Defense = defense;
            MovementSpeed = moveSpeed;
        }
    }
}
