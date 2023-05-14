using System;
using System.Collections.Generic;
using BlackSmith.Domain.Character;

namespace BlackSmith.Domain.Field
{
    // Field や Map, World の処理が似ている場合にも、それぞれのモジュールで役割が異なるため
    // 現時点での実装に共通点があった場合においても処理の共通化を行うことは望ましくない
    // 内部の関数内の実装等を抽象化し、共通のモジュールを内部で利用することは可能であるが
    // クラスの継承など、外部から見える形での依存関係の変更などは行うべきではない

    // 別に、Field等のエリア階層区分がゲームにより異なる事が想定される
    // その際の対応方針は別途考慮すべき

    // Chunk を内部にもち、Usecase等の外部モジュールからの I/O を担う

    // ライフサイクルを持つオブジェクト
    /// <summary>街やダンジョンを含むマップ単位</summary>
    public class Field
    {
        public FieldID ID { get; init; }

        public string Name { get; init; } // フィールド名

        public IReadOnlyCollection<CharacterID> CharacterIds => Chunk.CharacterIds;
        internal Chunk Chunk { get; private set; }

        public static Field ValueOf(FieldID id, string name, IReadOnlyCollection<CharacterID> ids)
        {
            return new Field(id, name, ids);
        }

        public static Field NameOf(string name)
        {
            return new Field(new FieldID(), name, new List<CharacterID>());
        }

        private Field(FieldID id, string name, IReadOnlyCollection<CharacterID> ids)
        {
            ID = id;
            Name = name;
            Chunk = new Chunk(ids);
        }

        /*  敵が倒される <- 敵のロジック
         *  Field, Chunkにその通知が来る
         *  N秒経過する <- 敵のロジック
         *  敵が復活する <- Usecaseに置く
         *  Field, Chunkにその通知が来る
         */

        /// <summary>アイテムやモンスターの補充を行う</summary>
        public void AddCharacter(CharacterID id)
        {
            Chunk = Chunk.AddCharacter(id);
        }

        // 倒されたという通知が流れてくる
        public void RemoveCharacter(CharacterID id)
        {
            Chunk = Chunk.RemoveCharacter(id);
        }
    }

    public class FieldID : BasicID
    {
        internal FieldID() : base(Guid.NewGuid()) { }

        internal FieldID(Guid id) : base(id) { }
    }

    // この区分けが必要かは検討すべき
    public enum FieldType
    {
        Village, // 街、安全圏内
        Abandoned, // 安全圏外
        Dungeon,
    }
}