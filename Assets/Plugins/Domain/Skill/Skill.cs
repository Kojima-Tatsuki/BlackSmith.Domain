using System;
using System.Collections.Generic;
using BlackSmith.Domain.Character.Player;

namespace BlackSmith.Domain.Skill
{
    /// <summary>スキル</summary>
    public class Skill
    {
        internal SkillName SkillName { get; }

        /// <summary>スキル熟練度</summary>
        internal SkillProficiency Proficiency { get; }

        internal SkillAcquisitionConditions AcquisitionConditions { get; }

        internal Skill(SkillName skillName, SkillExperience exp, SkillAcquisitionConditions acquisitionConditions)
        {
            SkillName = skillName ?? throw new ArgumentNullException(nameof(skillName));
            Proficiency = new SkillProficiency(exp) ?? throw new ArgumentNullException(nameof(exp));
            AcquisitionConditions = acquisitionConditions ?? throw new ArgumentNullException(nameof(acquisitionConditions));
        }

        /// <summary>スキルが取得できるか</summary>
        internal bool CanSkillAcquisition(SkillAcquisitionConditions requireParaeters) => AcquisitionConditions.CanSkillAcquisition(requireParaeters);
    }

    /// <summary>スキル名</summary>
    class SkillName
    {
        public string Value { get; }

        public SkillName(string value)
        {
            if (!IsValid(value))
                throw new AggregateException(nameof(value));

            Value = value;
        }

        private bool IsValid(string value)
        {
            if (value is null)
                return false;

            return true;
        }
    }

    /// <summary> 
    /// スキル熟練度
    /// </summary>
    /// <remarks>
    /// レベルの様に扱う, 内部にSkillExpを持つ
    /// 障害がない限りスキルレベルとも呼称する
    /// </remarks>
    class SkillProficiency
    {
        /// <summary>
        /// 熟練度 1-1000
        /// </summary>
        public int Value { get; }

        //private readonly int MaxValue = 1000;

        /// <summary>
        /// 累計獲得経験値
        /// </summary>
        public SkillExperience CumulativeExp { get; }

        internal SkillProficiency(SkillExperience exp = null!)
        {
            var calclator = new SkillExpCalculator();

            CumulativeExp = exp ?? new SkillExperience();

            Value = calclator.CurrentProficiency(CumulativeExp);
        }

        /// <summary>
        /// 経験値を加算する
        /// </summary>
        /// <param name="exp">加算する経験値量</param>
        /// <returns>加算後のスキル熟練度</returns>
        public SkillProficiency AddExp(SkillExperience exp)
        {
            return new SkillProficiency(CumulativeExp.Add(exp));
        }
    }

    /// <summary>スキルの取得条件</summary>
    class SkillAcquisitionConditions
    {
        public PlayerLevel Level { get; }
        public Strength Strength { get; }
        public Agility Agility { get; }

        internal SkillAcquisitionConditions(PlayerLevel level, Strength strength, Agility agility)
        {
            Level = level;
            Strength = strength;
            Agility = agility;
        }

        internal static SkillAcquisitionConditions FromDependentParams(LevelDependentParameters parameters)
        {
            return new SkillAcquisitionConditions(parameters.Level, parameters.STR, parameters.AGI);
        }

        public bool CanSkillAcquisition(SkillAcquisitionConditions condition)
        {
            if (condition.Level.Value < Level.Value)
                return false;

            return true;
        }
    }

    /// <summary>スキル経験値</summary>
    class SkillExperience
    {
        public int Value { get; }

        internal SkillExperience(int value = 0)
        {
            if (!IsValid(value))
                throw new ArgumentOutOfRangeException(nameof(value));

            Value = value;
        }

        private bool IsValid(int value)
        {
            if (value < 0)
                return false;

            return true;
        }

        public SkillExperience Add(SkillExperience other)
        {
            return new SkillExperience(Value + other.Value);
        }
    }

    /// <summary>スキル経験値計算機</summary>
    class SkillExpCalculator
    {
        // 1Lv -> 2Lv になるために行う動作の回数
        private readonly int InitRequireCount = 15;

        // 1Lv -> 2Lv になるために必要な経験値
        private readonly int InitRequireExp = 100;

        // レベル差倍率
        private readonly float LevelDifferenceMultiplier = 1.1f;

        /// <summary>
        /// 次のレベルまでに必要な経験値
        /// </summary>
        /// <param name="level">現在の熟練度</param>
        /// <returns>必要な経験値量</returns>
        public SkillExperience NeedToNextLevel(int level)
        {
            return new SkillExperience((int)(InitRequireExp * Math.Pow(LevelDifferenceMultiplier, level - 1)));
        }

        /// <summary>
        /// 1動作あたりの取得経験値量
        /// </summary>
        /// <param name="level">現在のレベル</param>
        /// <returns>取得できる経験値</returns>
        public SkillExperience ReceveExp(int level)
        {
            return new SkillExperience((int)Math.Round(InitRequireExp / InitRequireCount * Math.Pow(LevelDifferenceMultiplier, level - 1)));
        }

        /// <summary>
        /// レベルから累計取得経験値量を算出する
        /// </summary>
        /// <param name="level">現在の熟練度</param>
        /// <returns>累計経験値量</returns>
        public SkillExperience RequiredCumulativeExp(int level)
        {
            return new SkillExperience((int)((InitRequireExp * (1 - Math.Pow(LevelDifferenceMultiplier, level - 1)) / 1) - LevelDifferenceMultiplier));
        }

        /// <summary>
        /// 累計獲得経験値から現在の熟練度を算出する
        /// </summary>
        /// <param name="cumExp">累計獲得経験値</param>
        /// <returns>熟練度</returns>
        public int CurrentProficiency(SkillExperience cumExp)
        {
            return (int)Math.Log(1 - (cumExp.Value / InitRequireExp * (1 - LevelDifferenceMultiplier)), LevelDifferenceMultiplier) + 1;
        }
    }
}
