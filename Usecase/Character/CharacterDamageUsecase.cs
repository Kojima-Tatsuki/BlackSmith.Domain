using System;
using Microsoft.Extensions.DependencyInjection;
using BlackSmith.Domain.Character;
using BlackSmith.Domain.Player;
using BlackSmith.Repository.Interface;
using BlackSmith.Usecase.Interface;
using BlackSmith.Domain.CharacterObjects;

#nullable enable

namespace BlackSmith.Usecase.Character
{
    /// <summary>
    /// プレイヤーにダメージを与えるユースケース
    /// </summary>
    public class CharacterDamageUsecase
    {
        private readonly IPlayerRepository playerRepositoty;
        private readonly ICharacterRepositoty characterRepositoty;

        private readonly IOnPlayerHealthChangedEventHundler changedEventHundler;

        public CharacterDamageUsecase()
        {
            var provider = DIContainer.Instance.ServiceProvider;

            playerRepositoty = provider.GetRequiredService<IPlayerRepository>();
            characterRepositoty = provider.GetRequiredService<ICharacterRepositoty>();
            changedEventHundler = provider.GetRequiredService<IOnPlayerHealthChangedEventHundler>();
        }

        /// <summary>
        /// 攻め手の攻撃力ぶんのダメージを受け手に与える
        /// </summary>
        /// <param name="attackerId">攻め手のキャラクターID</param>
        /// <param name="recieverId">受け手のキャラクターID</param>
        public void TakeDamagePlayerByPlayer(PlayerID attackerId, PlayerID recieverId)
        {
            var attacker = playerRepositoty.FindByID(attackerId);
            var reciever = playerRepositoty.FindByID(recieverId);

            if (attacker is null)
                throw new ArgumentException($"そのようなIDのプレイヤーは存在しません ID : {attackerId}");
            if (reciever is null)
                throw new ArgumentException($"そのようなIDのプレイヤーは存在しません ID : {recieverId}");

            var levelGap = new LevelGapOfAttackerAndReceiver(reciever.Level, attacker.Level);

            var damage = new DamageValue(levelGap, attacker.Attack, reciever.Defence);

            reciever.TakeDamage(damage);

            playerRepositoty.UpdateCharacter(reciever);
        }

        /// <summary>
        /// キャラクターがプレイヤーにダメージを与える
        /// </summary>
        /// <param name="attackerId"></param>
        /// <param name="recieverId"></param>
        public void TakeDamagePlayerByCharacter(CharacterID attackerId, PlayerID recieverId)
        {
            var attacker = characterRepositoty.FindByID(attackerId);
            var reciever = playerRepositoty.FindByID(recieverId);

            if (attacker is null)
                throw new ArgumentException($"そのようなIDのキャラクターは存在しません ID : {attackerId}");
            if (reciever is null)
                throw new ArgumentException($"そのようなIDのプレイヤーは存在しません ID : {recieverId}");

            var levelGap = new LevelGapOfAttackerAndReceiver(reciever.Level, attacker.Level);

            var damage = new DamageValue(levelGap, attacker.Attack, reciever.Defence);

            reciever.TakeDamage(damage);

            var hp = reciever.HealthPoint.GetValues();
            changedEventHundler.OnChangedHealthPoint(hp.current, hp.max);

            playerRepositoty.UpdateCharacter(reciever);
        }

        public void TakeDamageCharacterByPlayer(PlayerID attackerId, CharacterID recieverId)
        {
            var attacker = playerRepositoty.FindByID(attackerId);
            var reciever = characterRepositoty.FindByID(recieverId);

            if (attacker is null)
                throw new ArgumentException($"そのようなIDのプレイヤーは存在しません ID : {attackerId}");
            if (reciever is null)
                throw new ArgumentException($"そのようなIDのキャラクターは存在しません ID : {recieverId}");

            var levelGap = new LevelGapOfAttackerAndReceiver(reciever.Level, attacker.Level);

            var damage = new DamageValue(levelGap, attacker.Attack, reciever.Defence);

            reciever.TakeDamage(damage);

            characterRepositoty.UpdateCharacter(reciever);
        }
    }
}