namespace BlackSmith.Domain.Character.Player
{
    /// <summary>
    /// キャラクターのレベル
    /// </summary>
    /// <remarks>Expでもってすべての計算を行っている</remarks>
    public record PlayerLevel : CharacterLevel
    {
        public int MaxValue { get; } = 100;

        /// <summary>
        /// 累計獲得経験値
        /// </summary>
        public Experience CumulativeExp { get; }

        internal PlayerLevel(Experience exp = null!) : base(Experience.CurrentLevel(exp ?? new Experience()))
        {
            CumulativeExp = exp ?? new Experience();
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