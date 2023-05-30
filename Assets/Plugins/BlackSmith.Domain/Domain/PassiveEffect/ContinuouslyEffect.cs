using BlackSmith.Domain.Character;
using Cysharp.Threading.Tasks;

namespace BlackSmith.Domain.PassiveEffect
{
    // 継続するエフェクト
    public interface ContinuouslyEffect
    {
        float ContinueTime { get; }

        UniTask DoEffect(CharacterID targetId);
    }

    // 継続時間の存在しないエフェクト
    public interface InstantEffect
    {

    }
}
