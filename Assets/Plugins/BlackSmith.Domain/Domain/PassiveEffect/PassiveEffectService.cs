using BlackSmith.Domain.Character;
using Cysharp.Threading.Tasks;
using UniRx;

namespace BlackSmith.Domain.PassiveEffect
{
    internal class PassiveEffectService
    {
        internal async UniTask PlayContinuouslyEffect(CharacterID targetId, ContinuouslyEffect effect)
        {
            await effect.DoEffect(targetId);
        }
    }
}
