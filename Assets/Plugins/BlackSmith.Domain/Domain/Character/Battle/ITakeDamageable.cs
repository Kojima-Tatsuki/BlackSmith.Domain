﻿using BlackSmith.Domain.Character.Battle;
using System;

namespace BlackSmith.Domain.CharacterObject.Interface
{
    internal interface ITakeDamageable
    {
        /// <summary>
        /// 体力を減らす、ダメージを与える
        /// </summary>
        /// <param name="damage">与えるダメージ</param>
        /// <returns>減少後の体力</returns>
        HealthPoint TakeDamage(DamageValue damage);

        /// <summary>
        /// 体力を回復させる
        /// </summary>
        /// <param name="value">回復する体力</param>
        /// <returns>回復後の体力</returns>
        /// <exception cref="ArgumentOutOfRangeException">valueに負数を入力した場合</exception>
        HealthPoint HealHealth(int value);
    }
}
