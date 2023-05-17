using System;

namespace BlackSmith.Domain.Character.Player
{
    /// <summary>
    /// 経験値量
    /// </summary>
    internal class Experience
    {
        // 1Lv -> 2Lv になる為に倒す敵の数
        private static readonly int InitKillRequirement = 5;

        // 1Lv -> 2Lv になる為に必要な経験値量
        private static readonly int InitExpRequirement = 100;

        // 1レベル上の敵を倒した時に貰える経験値の倍率
        private static readonly float LevelDifferenceMultiplier = 1.2f;

        internal int Value { get; }

        internal Experience(int value = 0)
        {
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
        internal Experience NeedToNextLevel(int level)
        {
            return new Experience((int)(InitExpRequirement * Math.Pow(LevelDifferenceMultiplier, level - 1)));
        }

        /// <summary>
        /// 敵1体あたりの取得経験値量
        /// </summary>
        /// <param name="level">敵のレベル</param>
        /// <returns>取得経験値量</returns>
        internal Experience ReceveExp(int level)
        {
            return new Experience((int)Math.Round(InitExpRequirement / InitKillRequirement * Math.Pow(LevelDifferenceMultiplier, level - 1)));
        }

        /// <summary>
        /// そのレベルに到達するまでに必要な経験値量を返す
        /// </summary>
        /// <remarks>累計取得経験値量</remarks>
        /// <param name="level"></param>
        /// <returns></returns>
        internal Experience RequiredCumulativeExp(int level)
        {
            return new Experience((int)(InitExpRequirement * (1 - Math.Pow(LevelDifferenceMultiplier, level - 1)) / 1 - LevelDifferenceMultiplier));
        }

        /// <summary>
        /// 累計獲得経験値から現在のレベルを算出する
        /// </summary>
        /// <param name="cumExp">累計取得経験値量</param>
        /// <returns>現在のレベル</returns>
        internal int CurrentLevel(Experience cumExp)
        {
            // I : InitExpRequirement
            // A : LevelDifferenceMultiplier
            // log_A ((1 / exp) * (1 - A) / I)
            return (int)Math.Log(1 - cumExp.Value / InitExpRequirement * (1 - LevelDifferenceMultiplier), LevelDifferenceMultiplier) + 1;
        }

        internal static Experience FromLevel(int level)
        {
            return new Experience((int)-((LevelDifferenceMultiplier - 1) * Math.Pow(LevelDifferenceMultiplier, 1 - level)) / InitExpRequirement);
        }
    }
}