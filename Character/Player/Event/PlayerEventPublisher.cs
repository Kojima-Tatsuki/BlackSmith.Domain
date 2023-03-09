using UniRx;

namespace BlackSmith.Domain.Player.Event
{
    // イベントの購読用
    public interface IHealthEventObservable
    {
        UniRx.IObservable<PlayerHealthChangedEvent> OnPlayerHealthChanged { get; }
        UniRx.IObservable<PlayerOnDeadEvent> OnPlayerDead { get; }
    }

    // イベントの発行用
    internal interface IHealthEventObserver
    {
        void SetChangedPlayerHealth(PlayerHealthChangedEvent changedEvent);

        void SetOnPlayerDead(PlayerOnDeadEvent deadEvent);
    }

    // ドメインサービスとして提供される
    // publicクラスとして定義する必要性があるかは疑問
    /// <summary>プレイヤーエンティティのイベントを提供する</summary>
    public class PlayerEventPublisher : IHealthEventObservable, IHealthEventObserver
    {
        /// <summary>
        /// プレイヤーの体力が変更された時に発行する
        /// </summary>
        public UniRx.IObservable<PlayerHealthChangedEvent> OnPlayerHealthChanged => onPlayerHealthChanged;
        private readonly Subject<PlayerHealthChangedEvent> onPlayerHealthChanged;

        /// <summary>
        /// プレイヤーが死亡した時に発行する
        /// </summary>
        public UniRx.IObservable<PlayerOnDeadEvent> OnPlayerDead => onPlayerDeadSubject;
        private readonly Subject<PlayerOnDeadEvent> onPlayerDeadSubject;


        public PlayerEventPublisher()
        {
            onPlayerHealthChanged = new Subject<PlayerHealthChangedEvent>();
            onPlayerDeadSubject = new Subject<PlayerOnDeadEvent>();
        }

        void IHealthEventObserver.SetChangedPlayerHealth(PlayerHealthChangedEvent changedEvent)
        {
            onPlayerHealthChanged.OnNext(changedEvent);
        }

        void IHealthEventObserver.SetOnPlayerDead(PlayerOnDeadEvent deadEvent)
        {
            onPlayerDeadSubject.OnNext(deadEvent);
        }
    }
}
