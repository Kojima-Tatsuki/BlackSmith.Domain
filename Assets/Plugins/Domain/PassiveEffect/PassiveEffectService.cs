using BlackSmith.Domain.Character;
using Cysharp.Threading.Tasks;
using UniRx;

namespace BlackSmith.Domain.PassiveEffect
{
    public class PassiveEffectService
    {
        public async UniTask PlayContinuouslyEffect(CharacterID targetId, ContinuouslyEffect effect)
        {
            await effect.DoEffect(targetId);
        }
    }
}
