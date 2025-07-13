using BlackSmith.Domain.Character;
using BlackSmith.Domain.Character.Battle;
using BlackSmith.Usecase.Interface;
using Cysharp.Threading.Tasks;
using System;

namespace BlackSmith.Usecase.Character.Battle
{
    /// <summary>
    /// キャラクターにダメージを与えるユースケース
    /// </summary>
    public class CharacterDamageUsecase
    {
        private IBattleCharacterEntityRepository PlayerRepository { get; }

        public CharacterDamageUsecase(IBattleCharacterEntityRepository playerRepository)
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
                throw new ArgumentException($"そのようなIDのキャラクターは存在しません ID : {attackerId}. (bHB0WXxR)");
            if (receiver is null)
                throw new ArgumentException($"そのようなIDのキャラクターは存在しません ID : {receiverId}. (DxYWTkAg6)");

            var levelGap = new LevelGapOfAttackerAndReceiver(receiver.Level, attacker.Level);

            var damage = new DamageValue(levelGap, attacker.Attack, receiver.Defense);

            receiver.TakeDamage(damage);

            await PlayerRepository.Update((receiver as BattleCharacterEntity) ?? throw new InvalidCastException($"受け手のキャラクターは、CharacterEntityではありません. (jAv4M0Pb)"));
        }
    }
}