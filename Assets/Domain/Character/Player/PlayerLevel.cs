using System;
using BlackSmith.Domain.Character.Interface;

namespace BlackSmith.Domain.Character.Player
{
    /// <summary>
    /// キャラクターのレベル
    /// </summary>
    /// <remarks>Expでもってすべての計算を行っている</remarks>
    public class PlayerLevel : ICharacterLevel
    {
        public int Value { get; }

        public int MaxValue { get; } = 100;

        /// <summary>
        /// 累計獲得経験値
        /// </summary>
        internal Experience CumulativeExp { get; }

        internal PlayerLevel(Experience exp = null!)
        {
            CumulativeExp = exp ?? new Experience();

            Value = CumulativeExp.CurrentLevel(CumulativeExp);
        }

        /// <summary>
        /// 最大レベルかどうかを返す
        /// </summary>
        /// <returns>最大レベルなら真を返す</returns>
        internal bool IsMaxLevel() => Value == MaxValue;

        /// <summary>
        /// 経験値を加える
        /// </summary>
        /// <param name="exp">加える経験値量</param>
        /// <returns>加えたあとのレベル</returns>
        internal PlayerLevel AddExp(Experience exp)
        {
            return new PlayerLevel(CumulativeExp.Add(exp));
        }
    }
}