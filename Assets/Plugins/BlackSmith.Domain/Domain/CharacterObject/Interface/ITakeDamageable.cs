using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackSmith.Domain.CharacterObject;

namespace BlackSmith.Domain.CharacterObject.Interface
{
    public interface ITakeDamageable
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
        HealthPoint HealHealth(int value);
    }
}
