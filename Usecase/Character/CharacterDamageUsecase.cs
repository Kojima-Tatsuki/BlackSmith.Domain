﻿using BlackSmith.Domain.Character.Player;
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
        private IOnPlayerHealthChangedEventHundler ChangedEventHundler { get; }

        public CharacterDamageUsecase(IPlayerRepository playerRepository, IOnPlayerHealthChangedEventHundler changedEventHundler)
        {
            PlayerRepository = playerRepository;
            ChangedEventHundler = changedEventHundler;
        }

        /// <summary>
        /// 攻め手の攻撃力ぶんのダメージを受け手に与える
        /// </summary>
        /// <param name="attackerId">攻め手のキャラクターID</param>
        /// <param name="recieverId">受け手のキャラクターID</param>
        public void TakeDamagePlayerByPlayer(PlayerID attackerId, PlayerID recieverId)
        {
            var attacker = PlayerRepository.FindByID(attackerId);
            var reciever = PlayerRepository.FindByID(recieverId);

            if (attacker is null)
                throw new ArgumentException($"そのようなIDのプレイヤーは存在しません ID : {attackerId}");
            if (reciever is null)
                throw new ArgumentException($"そのようなIDのプレイヤーは存在しません ID : {recieverId}");

            var levelGap = new LevelGapOfAttackerAndReceiver(reciever.Level, attacker.Level);

            var damage = new DamageValue(levelGap, attacker.Attack, reciever.Defense);

            reciever.TakeDamage(damage);

            PlayerRepository.UpdateCharacter(reciever);
        }
    }
}