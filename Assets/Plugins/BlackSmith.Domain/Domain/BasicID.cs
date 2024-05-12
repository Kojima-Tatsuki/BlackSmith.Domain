using System;

#nullable enable

namespace BlackSmith.Domain
{
    public abstract record BasicID
    {
        protected abstract string Prefix { get; }
        private Guid Value { get; }

        internal BasicID()
        {
            Value = Guid.NewGuid();
        }

        internal BasicID(string id)
        {
            if (!IsValid(id))
                throw new ArgumentException("指定したIDは適切な値ではありません");

            Value = GetGuid(id);
        }

        private bool IsValid(string id)
        {
            if (!id.StartsWith(Prefix))
                return false;
            if (!Guid.TryParse(id.Substring(Prefix.Length), out _))
                return false;

            return true;
        }

        private Guid GetGuid(string id) => Guid.Parse(id.Substring(Prefix.Length));

        public override string ToString() => Prefix + Value.ToString();
    }
}