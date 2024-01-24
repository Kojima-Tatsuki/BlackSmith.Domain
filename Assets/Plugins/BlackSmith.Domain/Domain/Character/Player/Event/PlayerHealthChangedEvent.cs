using BlackSmith.Domain.CharacterObject;

namespace BlackSmith.Domain.Character.Player.Event
{
    public class PlayerHealthChangedEvent : PlayerEventBase
    {
        public HealthPoint HealthPoint { get; }

        internal PlayerHealthChangedEvent(CharacterID id, HealthPoint health) : base(id)
        {
            HealthPoint = health;
        }

    }

    public class PlayerOnDeadEvent : PlayerEventBase
    {
        // 現在、死亡イベント独自の変数は無いのでコンストラクタの引数もない

        internal PlayerOnDeadEvent(CharacterID id) : base(id)
        {

        }
    }
}
