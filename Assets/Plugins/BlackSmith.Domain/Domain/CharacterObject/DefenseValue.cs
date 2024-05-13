using BlackSmith.Domain.Character.Battle;
using BlackSmith.Domain.Character.Player;
using System;
using System.Linq;

#nullable enable

namespace BlackSmith.Domain.CharacterObject
{
    /// <summary>
    /// 防御力
    /// </summary>
    public record DefenseValue
    {
        public int Value { get; }

        // internalで制限しているが、publicでも問題ないかもしれない
        internal int FromLevelDefence { get; }
        internal int FromWeaponDefense { get; }
        internal int FromArmorDefense { get; }
        internal int FromStatusEffectDefense { get; }

        internal DefenseValue(LevelDependentParameters levelParams, BattleEquipmentModule? equipmentModule = null, BattleStatusEffectModule? statusEffectModule = null)
        {
            FromLevelDefence = CheckVaild((levelParams.STR.Value + levelParams.AGI.Value) * 2);

            var eqModule = equipmentModule ?? new BattleEquipmentModule(null, null);
            FromWeaponDefense = eqModule.Weapon?.Defense?.Value ?? 0;
            FromArmorDefense = eqModule.Armor?.Defense?.Value ?? 0;
            
            var statModule = statusEffectModule ?? new BattleStatusEffectModule();
            FromStatusEffectDefense = statModule.StatusEffects.Sum(effect => effect.StatusModel.Defense);

            Value = CheckVaild(FromLevelDefence + FromWeaponDefense + FromArmorDefense + FromStatusEffectDefense);
        }

        private int CheckVaild(int value)
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException($"防御力には1以上の値を入力してください, value : {value}");

            return value;
        }

        // public アクセサのValueのみを出力
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}