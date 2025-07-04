using BlackSmith.Domain.Character.Interface;
using System;

namespace BlackSmith.Domain.Character.Player
{
    // 名前やレベルなど、キャラクターの基本情報を保持する
    public class CommonCharacterEntity : ICharacterEntity, IEquatable<CommonCharacterEntity>
    {
        public CharacterID ID { get; }
        public CharacterName Name { get; private set; }
        public CharacterLevel Level { get; private set; }

        /// <summary>
        /// キャラクターエンティティのインスタンス化を行う
        /// </summary>
        /// <remarks>再利用する際は、ファクトリーで変更メソッドを呼び出す？</remarks>
        /// <param name="command"></param>
        internal CommonCharacterEntity(CommonCharacterReconstructCommand command)
        {
            ID = command.Id;
            Name = command.Name;
            Level = command.Level;
        }

        /// <summary> 名前の変更を行う </summary>
        /// <param name="name">変更する名前</param>
        void ICharacterEntity.ChangeName(CharacterName name)
        {
            Name = name ?? throw new ArgumentNullException("Not found CharacterName. (O94YoFRG)");
        }

        public CommonCharacterReconstructCommand GetReconstructCommand() => new(ID, Name, Level);

        /// <summary>
        /// 内部情報を文字列として表示する
        /// </summary>
        public override string ToString()
        {
            var result = "";
            result += $"ItemName : {Name.Value}\n";
            result += $"ID : {ID}\n";
            result += $"Level : {Level.Value}";

            return result;
        }

        public bool Equals(CommonCharacterEntity other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            // IDの一致のみで判定
            return ID.Equals(other.ID);
        }
    }
}