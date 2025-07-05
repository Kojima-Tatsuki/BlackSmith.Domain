using BlackSmith.Domain.Character.Player;
using Newtonsoft.Json;
using System;

#nullable enable

namespace BlackSmith.Domain.Character
{
    /// <summary>
    /// プレイヤーのレベル
    /// </summary>
    /// <remarks>Expでもってすべての計算を行っている</remarks>
    public record CharacterLevel
    {
        public const int MaxValue = 100;

        /// <summary>
        /// 累計獲得経験値
        /// </summary>
        public Experience CumulativeExp { get; }

        [JsonConstructor]
        internal CharacterLevel(Experience? cumulativeExp = null) : base()
        {
            CumulativeExp = cumulativeExp ?? new Experience();
        }

        public int Value => Experience.CurrentLevel(CumulativeExp);

        private protected bool IsValid(int value)
        {
            if (value <= 0)
                return false;

            return true;
        }

        /// <summary>
        /// 経験値を加える
        /// </summary>
        /// <param name="exp">加える経験値量</param>
        /// <returns>加えたあとのレベル</returns>
        internal CharacterLevel AddExp(Experience exp)
        {
            return new CharacterLevel(CumulativeExp.Add(exp));
        }

        /// <summary>
        /// 最大レベルかどうかを返す
        /// </summary>
        /// <returns>最大レベルなら真を返す</returns>
        internal bool IsMaxLevel() => Value == MaxValue;

        // TODO: これはLevelのロジックではなく、Skillのロジックなので移動させる
        /// <summary>取得できるスキル数を返す</summary>
        /// <returns></returns>
        internal int GetNumberOfSkillsAvailable()
        {
            // 初期状態で2つ
            // 6, 12レベルで1つずつ増える
            // 以降、10レベル毎に1つ増える

            if (Value < 12)
                return 2 + (int)Math.Floor((double)Value / 6);

            return 3 + (int)Math.Floor((double)Value / 10);
        }
    }
}