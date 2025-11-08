using System;
using NUnit.Framework;

#nullable enable

namespace BlackSmith.Domain
{
    // BasicIDのテスト用モッククラス
    public record MockID : BasicID
    {
        protected override string Prefix => "mock_";
        public MockID() : base() { }
        public MockID(string value) : base(value) { }
    }

    [TestFixture]
    public class BasicIDTest
    {
        [Test(Description = "引数なしコンストラクタでインスタンス化できること")]
        public void Constructor_NoArg_CreatesInstance()
        {
            var id = new MockID();
            Assert.That(id.Value.StartsWith("mock_"));
            Assert.That(Guid.TryParse(id.Value.Substring("mock_".Length), out _), Is.True);
        }

        [Test(Description = "value引数でインスタンス化できること")]
        public void Constructor_WithValue_CreatesInstance()
        {
            var id1 = new MockID();
            var id2 = new MockID(id1.Value);
            Assert.That(id1.Value, Is.EqualTo(id2.Value));
        }

        [Test(Description = "不正なvalueで例外が発生すること")]
        public void Constructor_WithInvalidValue_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => new MockID("invalid_value"));
            Assert.Throws<ArgumentException>(() => new MockID("mock_")); // Guid部分がない
            Assert.Throws<ArgumentException>(() => new MockID(null!));
        }
    }
}
