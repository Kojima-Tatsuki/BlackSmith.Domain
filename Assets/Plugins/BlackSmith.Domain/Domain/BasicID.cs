using Newtonsoft.Json;
using System;

#nullable enable

namespace BlackSmith.Domain
{
    public abstract record BasicID
    {
        protected abstract string Prefix { get; }

        public string Value => Prefix + guid;
        private Guid guid { get; }

        internal BasicID()
        {
            guid = Guid.NewGuid();
        }

        internal BasicID(string value)
        {
            if (!IsValid(value))
                throw new ArgumentException($"指定したIDは適切な値ではありません. value: {value}");

            guid = GetGuid(value);
        }

        private bool IsValid(string id)
        {
            if (id is null)
                return false;
            if (!id.StartsWith(Prefix))
                return false;
            if (!Guid.TryParse(id.Substring(Prefix.Length), out _))
                return false;

            return true;
        }

        private Guid GetGuid(string id) => Guid.Parse(id.Substring(Prefix.Length));

        public override string ToString() => Prefix + guid;
    }
}