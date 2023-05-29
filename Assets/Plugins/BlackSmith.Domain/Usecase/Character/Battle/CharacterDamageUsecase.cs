using BlackSmith.Domain.Character;
using BlackSmith.Domain.Character.Battle;
using BlackSmith.Domain.Character.Player;
using BlackSmith.Domain.CharacterObject;
using BlackSmith.Usecase.Interface;
using System;

namespace BlackSmith.Usecase.Character.Battle
{
    /// <summary>
    /// プレイヤーにダメージを与えるユースケース
    /// </summary>
    public class CharacterDamageUsecase
    {
        private IPlayerRepository PlayerRepository { get; }

        public CharacterDamageUsecase(IPlayerRepository playerRepository)
        {
            PlayerRepository = playerRepository;
        }

        /// <summary>
        /// 攻め手の攻撃力ぶんのダメージを受け手に与える
        /// </summary>
        /// <param name="attackerId">攻め手のキャラクターID</param>
        /// <param name="receiverId">受け手のキャラクターID</param>
        public void TakeDamagePlayerByPlayer(CharacterID attackerId, CharacterID receiverId)
        {
            var attacker = PlayerRepository.FindByID(attackerId);
            var receiver = PlayerRepository.FindByID(receiverId) as IBattleCharacter;

            if (attacker is null)
                throw new ArgumentException($"そのようなIDのプレイヤーは存在しません ID : {attackerId}");
            if (receiver is null)
                throw new ArgumentException($"そのようなIDのプレイヤーは存在しません ID : {receiverId}");

            var levelGap = new LevelGapOfAttackerAndReceiver(receiver.Level, attacker.Level);

            var damage = new DamageValue(levelGap, attacker.Attack, receiver.Defense);

            receiver.TakeDamage(damage);

            PlayerRepository.UpdateCharacter((receiver as PlayerEntity) ?? throw new InvalidOperationException(nameof(receiver)));
        }
    }
}