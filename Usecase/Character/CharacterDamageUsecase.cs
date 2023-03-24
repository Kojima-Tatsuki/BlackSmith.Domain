using BlackSmith.Domain.Character.Player;
using BlackSmith.Domain.CharacterObject;
using BlackSmith.Usecase.Interface;

namespace BlackSmith.Usecase.Character
{
    /// <summary>
    /// プレイヤーにダメージを与えるユースケース
    /// </summary>
    public class CharacterDamageUsecase
    {
        private IPlayerRepository PlayerRepositoty { get; init; }
        private IOnPlayerHealthChangedEventHundler ChangedEventHundler { get; init; }

        public CharacterDamageUsecase(IPlayerRepository playerRepository, IOnPlayerHealthChangedEventHundler changedEventHundler)
        {
            PlayerRepositoty = playerRepository;
            ChangedEventHundler = changedEventHundler;
        }

        /// <summary>
        /// 攻め手の攻撃力ぶんのダメージを受け手に与える
        /// </summary>
        /// <param name="attackerId">攻め手のキャラクターID</param>
        /// <param name="recieverId">受け手のキャラクターID</param>
        public void TakeDamagePlayerByPlayer(PlayerID attackerId, PlayerID recieverId)
        {
            var attacker = PlayerRepositoty.FindByID(attackerId);
            var reciever = PlayerRepositoty.FindByID(recieverId);

            if (attacker is null)
                throw new ArgumentException($"そのようなIDのプレイヤーは存在しません ID : {attackerId}");
            if (reciever is null)
                throw new ArgumentException($"そのようなIDのプレイヤーは存在しません ID : {recieverId}");

            var levelGap = new LevelGapOfAttackerAndReceiver(reciever.Level, attacker.Level);

            var damage = new DamageValue(levelGap, attacker.Attack, reciever.Defence);

            reciever.TakeDamage(damage);

            PlayerRepositoty.UpdateCharacter(reciever);
        }
    }
}