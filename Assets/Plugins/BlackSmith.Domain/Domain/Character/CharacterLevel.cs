﻿using Newtonsoft.Json;
using System;

namespace BlackSmith.Domain.Character
{
    // Expを利用しないため、レベルに変動を起こさせない
    public record CharacterLevel
    {
        public int Value { get; }

        [JsonConstructor]
        internal CharacterLevel(int value)
        {
            if (!IsVaild(value))
                throw new ArgumentException($"0以下の値はレベルとして扱えません, value : {value}. (cz7lNbVb)");

            Value = value;
        }

        private bool IsVaild(int value)
        {
            if (value <= 0)
                return false;

            return true;
        }

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