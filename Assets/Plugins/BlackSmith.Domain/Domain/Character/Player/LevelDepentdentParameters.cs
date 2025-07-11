﻿using Newtonsoft.Json;
using System;

#nullable enable

namespace BlackSmith.Domain.Character.Player
{
    /// <summary>
    /// レベルを元に変動するパラメータを格納する
    /// </summary>
    /// <remarks>Value Object</remarks>
    public record LevelDependentParameters
    {
        /// <summary>
        /// 現在のキャラクターのレベル
        /// </summary>
        public CharacterLevel Level { get; }

        public Strength STR { get; }

        public Agility AGI { get; }

        /// <summary>
        /// 1レベル上昇する毎に貰えるステータス上昇ポイントの数
        /// </summary>
        private const int ReceivedPointsPerLevelIncrease = 3;

        /// <summary>
        /// 初期値でもってインスタンス化を行う
        /// </summary>
        internal LevelDependentParameters()
        {
            Level = new CharacterLevel();
            STR = new Strength(1);
            AGI = new Agility(1);
        }

        /// <summary>
        /// 値を指定してインスタンス化を行う
        /// </summary>
        /// <param name="level"></param>
        /// <param name="str"></param>
        /// <param name="agi"></param>
        [JsonConstructor]
        internal LevelDependentParameters(CharacterLevel level, Strength str, Agility agi)
        {
            Level = level;
            if (GetLevelDependParamPoint(level) < str.Value + agi.Value)
                throw new ArgumentException($"指定したSTR, AGIは割当可能量を超過しています STR: {str.Value}, AGI: {agi.Value}");
            STR = str;
            AGI = agi;
        }

        /// <summary>
        /// まだ使用されていないステータス上昇ポイント量を返す
        /// </summary>
        /// <returns></returns>
        internal int GetRemainingParamPoint()
        {
            return Level.Value * ReceivedPointsPerLevelIncrease - (STR.Value + AGI.Value);
        }

        /// <summary>
        /// ステータス上昇ポイントを加える
        /// </summary>
        /// <param name="str">筋力に加算するポイント数</param>
        /// <param name="agi">俊敏力に加算するポイント数</param>
        /// <returns>加算後のステータス</returns>
        internal LevelDependentParameters AddParamPoint(int str, int agi)
        {
            if (GetRemainingParamPoint() < str + agi)
                throw new ArgumentException($"指定された上昇量が加算できる値を超えています, " +
                    $"canAdd : {GetRemainingParamPoint()}, " +
                    $"str : {str}, " +
                    $"agi : {agi}");

            return new LevelDependentParameters(
                Level,
                new Strength(STR.Value + str),
                new Agility(AGI.Value + agi));
        }

        /// <summary>
        /// 指定したレベルでのステータス上昇ポイントの総量を返す
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        internal static int GetLevelDependParamPoint(CharacterLevel level)
        {
            if (level is null)
                throw new ArgumentNullException(nameof(level));

            return level.Value * ReceivedPointsPerLevelIncrease;
        }
    }

    /// <summary>
    /// 筋力
    /// </summary>
    public record Strength
    {
        public int Value => value.Value;
        private readonly BasePlayerParameter value;

        [JsonConstructor]
        internal Strength(int value)
            => this.value = new BasePlayerParameter(value);
    }

    /// <summary>
    /// 俊敏性
    /// </summary>
    public record Agility
    {
        public int Value => value.Value;
        private readonly BasePlayerParameter value;

        [JsonConstructor]
        internal Agility(int value)
            => this.value = new BasePlayerParameter(value);
    }

    /// <summary>
    /// 基礎ステータスの基底クラス
    /// </summary>
    internal record BasePlayerParameter
    {
        public int Value { get; }

        [JsonConstructor]
        internal BasePlayerParameter(int value)
        {
            if (!IsValid(value))
                throw new ArgumentException($"valueに不正な値が入力されました, value: {value}");

            Value = value;
        }

        private static bool IsValid(int value)
        {
            if (value < 1)
                return false;
            return true;
        }
    }
}