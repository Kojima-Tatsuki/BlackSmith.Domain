using System;
using System.Collections.Generic;

namespace BlackSmith.Domain.Field
{
    /// <summary>街やダンジョンを含むマップ単位</summary>
    internal class Field
    {
        public FieldID ID { get; }

        public string Name { get; }

        public Field(FieldID id, string name)
        {
            ID = id;
            Name = name;
        }
    }

    public class FieldID: BasicID
    {
        public FieldID(Guid id): base(id) { }
    }

    public enum FieldType
    {
        Village, // 街、安全圏内
        Abandoned, // 安全圏外
        Dungeon,
    }
}
