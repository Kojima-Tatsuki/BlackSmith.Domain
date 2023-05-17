using BlackSmith.Domain.Character.Player;
using BlackSmith.Domain.CharacterObject;
using BlackSmith.Usecase.Interface;
using System;

namespace BlackSmith.Usecase.Character
{
    /// <summary>
    /// プレイヤーにダメージを与えるユースケース
    /// </summary>
    public class CharacterDamageUsecase
    {
        private IPlayerRepository PlayerRepository { get; }
        private IOnPlayerHealthChangedEventHundler ChangedEventHandler { get; }

        public CharacterDamageUsecase(IPlayerRepository playerRepository, IOnPlayerHealthChangedEventHundler changedEventHundler)
        {
            PlayerRepository = playerRepository;
            ChangedEventHandler = changedEventHundler;
        }

        /// <summary>
        /// 攻め手の攻撃力ぶんのダメージを受け手に与える
        /// </summary>
        /// <param name="attackerId">攻め手のキャラクターID</param>
        /// <param name="receiverId">受け手のキャラクターID</param>
        public void TakeDamagePlayerByPlayer(PlayerID attackerId, PlayerID receiverId)
        {
            var attacker = PlayerRepository.FindByID(attackerId);
            var receiver = PlayerRepository.FindByID(receiverId);

            if (attacker is null)
                throw new ArgumentException($"そのようなIDのプレイヤーは存在しません ID : {attackerId}");
            if (receiver is null)
                throw new ArgumentException($"そのようなIDのプレイヤーは存在しません ID : {receiverId}");

            var levelGap = new LevelGapOfAttackerAndReceiver(receiver.Level, attacker.Level);

            var damage = new DamageValue(levelGap, attacker.Attack, receiver.Defense);

            receiver.TakeDamage(damage);

            PlayerRepository.UpdateCharacter(receiver);
        }
    }
}