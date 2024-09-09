using BlackSmith.Domain.Character;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlackSmith.Domain.Field
{
    // フィールド上に存在する地形ではないデータを扱う
    internal class Chunk
    {
        public Guid ChunkId { get; }

        public IReadOnlyCollection<CharacterID> CharacterIds { get; }

        // コンストラクタのスコープに関しては、リポジトリの実装に依存する
        internal Chunk(IReadOnlyCollection<CharacterID> exists)
        {
            ChunkId = Guid.NewGuid();
            CharacterIds = exists;
        }

        internal Chunk RemoveCharacter(CharacterID id)
        {
            var list = CharacterIds
                .Where(x => x != id)
                .ToList();

            return new(list);
        }

        internal Chunk AddCharacter(CharacterID id)
        {
            var list = CharacterIds
                .ToList()
                .Append(id)
                .ToList();

            return new(list);
        }
    }
}
