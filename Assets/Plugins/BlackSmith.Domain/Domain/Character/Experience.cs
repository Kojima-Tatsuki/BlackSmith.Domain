using Newtonsoft.Json;
using System;

namespace BlackSmith.Domain.Character
{
    /// <summary>
    /// 経験値量
    /// </summary>
    public record Experience
    {
        // 1Lv -> 2Lv になる為に倒す敵の数
        private static readonly int InitKillRequirement = 5;

        // 1Lv -> 2Lv になる為に必要な経験値量
        private static readonly int InitExpRequirement = 10;

        // 1レベル上の敵を倒した時に貰える経験値の倍率
        private static readonly float LevelDifferenceMultiplier = 1.1f;

        public int Value { get; }

        [JsonConstructor]
        internal Experience(int value = 0)
        {
            if (!IsValid(value))
                throw new ArgumentOutOfRangeException("経験値として適さない値が投入されました。");

            Value = value;
        }

        /// <summary>
        /// 経験値を加える
        /// </summary>
        /// <param name="other">加える経験値</param>
        /// <returns>加えた後の経験値</returns>
        internal Experience Add(Experience other)
        {
            return new Experience(Value + other.Value);
        }

        /// <summary>
        /// 次のレベルまでに必要な経験値
        /// </summary>
        /// <param name="level">現在のレベル</param>
        /// <returns></returns>
        internal static Experience NeedToNextLevel(int level)
        {
            return new Experience((int)(InitExpRequirement * Math.Pow(LevelDifferenceMultiplier, level - 1)));
        }

        /// <summary>
        /// 敵1体あたりの取得経験値量
        /// </summary>
        /// <param name="level">敵のレベル</param>
        /// <returns>取得経験値量</returns>
        internal static Experience ReceiveExp(int level)
        {
            // 次のレベルまでに必要な経験値量 / 倒す必要のある敵数
            return new Experience((int)(InitExpRequirement * Math.Pow(LevelDifferenceMultiplier, level - 1) / InitKillRequirement));
        }

        /// <summary>
        /// そのレベルに到達するまでに必要な経験値量を返す
        /// </summary>
        /// <remarks>累計取得経験値量</remarks>
        /// <param name="level"></param>
        /// <returns></returns>
        internal static Experience RequiredCumulativeExp(int level)
        {
            if (level <= 0 || level > 100)
                throw new ArgumentOutOfRangeException("レベルは1～100の範囲で入力してください。");

            // I : InitExpRequirement
            // A : LevelDifferenceMultiplier
            // I * (1 - A ^ (level - 1)) / (1 - A)
            return new Experience((int)(InitExpRequirement * (1 - Math.Pow(LevelDifferenceMultiplier, level - 1)) / (1 - LevelDifferenceMultiplier)));
        }

        /// <summary>
        /// 累計獲得経験値から現在のレベルを算出する
        /// </summary>
        /// <param name="cumExp">累計取得経験値量</param>
        /// <returns>現在のレベル</returns>
        internal static int CurrentLevel(Experience cumExp)
        {
            // I : InitExpRequirement
            // A : LevelDifferenceMultiplier
            // log_A (1 - (exp / I * (1 - A)) + 1

            // 丸め誤差の関係で正確な値が出ない
            var result = (int)Math.Log(1 - (cumExp.Value * (1 - (double)LevelDifferenceMultiplier) / InitExpRequirement), LevelDifferenceMultiplier) + 1;

            if (RequiredCumulativeExp(result).Value < cumExp.Value && RequiredCumulativeExp(result + 1).Value <= cumExp.Value)
                return ++result;
            return result;
        }

        internal static double Cld(Experience cumExp)
        {
            return Math.Log(1 - (cumExp.Value * (1 - (double)LevelDifferenceMultiplier) / InitExpRequirement), LevelDifferenceMultiplier) + 1;
        }

        private bool IsValid(int value)
        {
            if (value < 0)
                return false;
            return true;
        }
    }
}