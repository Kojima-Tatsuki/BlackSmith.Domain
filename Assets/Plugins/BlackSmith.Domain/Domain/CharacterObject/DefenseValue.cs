using System;
using System.Linq;
using BlackSmith.Domain.Character.Battle;
using BlackSmith.Domain.Character.Player;

namespace BlackSmith.Domain.CharacterObject
{
    /// <summary>
    /// �h���
    /// </summary>
    public class DefenseValue
    {
        public int Value { get; }

        private int FromLevelAttack { get; }
        private int WeaponAttack { get; }
        private int ArmorAttack { get; }
        private int StatusEffect { get; }

        internal DefenseValue(LevelDependentParameters levelParams, BattleEquipmentModule equipmentModule, BlattleStatusEffectModule statusEffectModule)
        {
            FromLevelAttack = CheckVaild((levelParams.STR.Value + levelParams.AGI.Value) * 2);
            WeaponAttack = CheckVaild(equipmentModule.Weapon?.Defense?.Value ?? 0);
            ArmorAttack = CheckVaild(equipmentModule.Armor?.Defense?.Value ?? 0);
            StatusEffect = CheckVaild(statusEffectModule.StatusEffects.Sum(effect => effect.StatusModel.Defense));

            Value = CheckVaild(FromLevelAttack + WeaponAttack + ArmorAttack + StatusEffect);
        }

        private int CheckVaild(int value)
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException($"�h��͂ɂ�1�ȏ�̒l����͂��Ă�������, value : {value}");

            return value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        internal DefenseDetailModel GetDetail()
        {
            return new DefenseDetailModel(FromLevelAttack, WeaponAttack, ArmorAttack, StatusEffect);
        }
    }

    public class DefenseDetailModel
    {
        public int Level { get; }
        public int Weapon { get; }
        public int Armor { get; }
        public int StatusEffect { get; }

        internal DefenseDetailModel(int level, int weapon, int armor, int statusEffect)
        {
            Level = level;
            Weapon = weapon;
            Armor = armor;
            StatusEffect = statusEffect;
        }
    }
}