using BlackSmith.Domain.Character.Battle;
using BlackSmith.Domain.Character.Player;
using System;
using System.Linq;

#nullable enable

namespace BlackSmith.Domain.CharacterObject
{
    /// <summary>
    /// 攻撃力
    /// </summary>
    public record AttackValue
    {
        public int Value { get; }

        internal int FromLevelAttack { get; }
        internal int FromWeaponAttack { get; }
        internal int FromArmorAttack { get; }
        internal int FromStatusEffectAttack { get; }

        internal AttackValue(LevelDependentParameters levelParams, BattleEquipmentModule? equipmentModule, BattleStatusEffectModule? statusEffectModule)
        {
            FromLevelAttack = CheckVaild((levelParams.STR.Value + levelParams.AGI.Value) * 2); // ここは必ず1以上の値

            var eqModule = equipmentModule ?? new BattleEquipmentModule(null, null);
            FromWeaponAttack = eqModule.Weapon?.Attack?.Value ?? 0;
            FromArmorAttack = eqModule.Armor?.Attack?.Value ?? 0;

            var statModule = statusEffectModule ?? new BattleStatusEffectModule();
            FromStatusEffectAttack = statModule.StatusEffects.Sum(effect => effect.StatusModel.Attack);

            Value = CheckVaild(FromLevelAttack + FromWeaponAttack + FromArmorAttack + FromStatusEffectAttack);
        }

        private int CheckVaild(int value)
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException($"攻撃力には1以上の値を入力してください, value : {value}. (eaGdKodc4)");

            return value;
        }

        public override string ToString() => Value.ToString();
    }
}
