using BlackSmith.Domain.Character;
using BlackSmith.Domain.Character.Battle;
using BlackSmith.Usecase.Interface;
using Cysharp.Threading.Tasks;
using System;

namespace BlackSmith.Usecase.Character.Battle
{
    /// <summary>
    /// プレイヤーにダメージを与えるユースケース
    /// </summary>
    public class CharacterDamageUsecase
    {
        private IPlayerBattleEntityRepository PlayerRepository { get; }

        public CharacterDamageUsecase(IPlayerBattleEntityRepository playerRepository)
        {
            PlayerRepository = playerRepository;
        }

        /// <summary>
        /// 攻め手の攻撃力ぶんのダメージを受け手に与える
        /// </summary>
        /// <param name="attackerId">攻め手のキャラクターID</param>
        /// <param name="receiverId">受け手のキャラクターID</param>
        public async UniTask TakeDamagePlayerByPlayer(CharacterID attackerId, CharacterID receiverId)
        {
            var attacker = await PlayerRepository.FindByID(attackerId);
            var receiver = await PlayerRepository.FindByID(receiverId) as IBattleCharacter;

            if (attacker is null)
                throw new ArgumentException($"そのようなIDのプレイヤーは存在しません ID : {attackerId}. (bHB0WXxR)");
            if (receiver is null)
                throw new ArgumentException($"そのようなIDのプレイヤーは存在しません ID : {receiverId}. (DxYWTkAg6)");

            var levelGap = new LevelGapOfAttackerAndReceiver(receiver.Level, attacker.Level);

            var damage = new DamageValue(levelGap, attacker.Attack, receiver.Defense);

            receiver.TakeDamage(damage);

            await PlayerRepository.UpdateCharacter((receiver as PlayerBattleEntity) ?? throw new InvalidCastException($"受け手のキャラクターは、PlayerEntityではありません. (jAv4M0Pb)"));
        }
    }
}