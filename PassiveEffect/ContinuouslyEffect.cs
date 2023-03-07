using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;

namespace BlackSmith.Domain.PassiveEffect
{
    // 継続するエフェクト
    abstract class ContinuouslyEffect
    {
        public float ContinueTime { get; }

        public abstract UniTask DoEffect();
    }

    // 継続時間の存在しないエフェクト
    class InstantEffect
    {

    }
}
