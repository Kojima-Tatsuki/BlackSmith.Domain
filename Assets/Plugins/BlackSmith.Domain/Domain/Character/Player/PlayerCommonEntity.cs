using BlackSmith.Domain.Character.Interface;
using System;

namespace BlackSmith.Domain.Character.Player
{
    // 名前や種族など、プレイヤーの基本情報を保持する
    public class PlayerCommonEntity : ICharacterEntity, IEquatable<PlayerCommonEntity>
    {
        public CharacterID ID { get; }
        public PlayerName Name { get; private set; }

        CharacterLevel ICharacterEntity.Level => Level;
        public PlayerLevel Level { get; private set; }

        /// <summary>
        /// プレイヤーエンティティのインスタンス化を行う
        /// </summary>
        /// <remarks>再利用する際は、ファクトリーで変更メソッドを呼び出す？</remarks>
        /// <param name="id"></param>
        internal PlayerCommonEntity(PlayerCommonReconstructCommand command)
        {
            ID = command.Id;
            Name = command.Name;
            Level = command.Level;
        }

        /// <summary> 名前の変更を行う </summary>
        /// <param name="name">変更する名前</param>
        void ICharacterEntity.ChangeName(PlayerName name)
        {
            Name = name ?? throw new ArgumentNullException("Not found PlayerName. (O94YoFRG)");
        }

        public PlayerCommonReconstructCommand GetReconstractCommand() => new(ID, Name, Level);

        /// <summary>
        /// 内部情報を文字列として表示する
        /// </summary>
        public override string ToString()
        {
            var result = "";
            result += $"Name : {Name.Value}\n";
            result += $"ID : {ID}\n";
            result += $"Level : {Level.Value}";

            return result;
        }

        public bool Equals(PlayerCommonEntity other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            // IDの一致のみで判定
            return ID.Equals(other.ID);
        }
    }
}