using System;
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

        internal AttackValue(LevelDependentParameters levelParams, BattleEquipmentModule equipmentModule)
        {
            FromLevelAttack = CheckVaild((levelParams.STR.Value + levelParams.AGI.Value) * 2);
            WeaponAttack = CheckVaild(equipmentModule.Weapon?.Attack.Value ?? 0);
            ArmorAttack = CheckVaild(equipmentModule.Armor?.Attack?.Value ?? 0);

            Value = CheckVaild(FromLevelAttack + WeaponAttack + ArmorAttack);
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
            return new AttackDetailModel(FromLevelAttack, WeaponAttack, ArmorAttack);
        }
    }

    public class AttackDetailModel
    {
        public int Level { get; }
        public int Weapon { get; }
        public int Armor { get; }

        internal AttackDetailModel(int level, int weapon, int armor)
        {
            Level = level;
            Weapon = weapon;
            Armor = armor;
        }
    }
}
