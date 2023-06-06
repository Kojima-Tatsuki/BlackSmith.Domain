using System;
using System.Linq;
using BlackSmith.Domain.Character.Battle;
using BlackSmith.Domain.Character.Player;

namespace BlackSmith.Domain.CharacterObject
{
    /// <summary>
    /// 攻撃力
    /// </summary>
    public class AttackValue
    {
        public int Value { get; }

        private int FromLevelAttack { get; }
        private int WeaponAttack { get; }
        private int ArmorAttack { get; }
        private int StatusEffect { get; }

        internal AttackValue(LevelDependentParameters levelParams, BattleEquipmentModule equipmentModule, BlattleStatusEffectModule statusEffectModule)
        {
            FromLevelAttack = CheckVaild((levelParams.STR.Value + levelParams.AGI.Value) * 2); // ここは必ず1以上の値
            WeaponAttack = equipmentModule.Weapon?.Attack.Value ?? 0;
            ArmorAttack = equipmentModule.Armor?.Attack?.Value ?? 0;
            StatusEffect = statusEffectModule.StatusEffects.Sum(effect => effect.StatusModel.Attack);

            Value = CheckVaild(FromLevelAttack + WeaponAttack + ArmorAttack + StatusEffect);
        }

        private int CheckVaild(int value)
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException($"攻撃力には1以上の値を入力してください, value : {value}");

            return value;
        }

        public override string ToString() => Value.ToString();

        internal AttackDetailModel GetDetail()
        {
            return new AttackDetailModel(FromLevelAttack, WeaponAttack, ArmorAttack, StatusEffect);
        }
    }

    public class AttackDetailModel
    {
        public int Level { get; }
        public int Weapon { get; }
        public int Armor { get; }
        public int StatusEffect { get; }

        internal AttackDetailModel(int level, int weapon, int armor, int effect)
        {
            Level = level;
            Weapon = weapon;
            Armor = armor;
            StatusEffect = effect;
        }
    }
}
