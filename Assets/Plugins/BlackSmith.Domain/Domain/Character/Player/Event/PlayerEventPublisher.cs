using R3;

namespace BlackSmith.Domain.Character.Player.Event
{
    // イベントの購読用
    public interface IHealthEventObservable
    {
        Observable<PlayerHealthChangedEvent> OnPlayerHealthChanged { get; }
        Observable<PlayerOnDeadEvent> OnPlayerDead { get; }
    }

    // イベントの発行用
    internal interface IHealthEventObserver
    {
        void SetChangedPlayerHealth(PlayerHealthChangedEvent changedEvent);

        void SetOnPlayerDead(PlayerOnDeadEvent deadEvent);
    }

    // ドメインサービスとして提供される
    // publicクラスとして定義する必要性があるかは疑問
    /// <summary>キャラクターエンティティのイベントを提供する</summary>
    public class PlayerEventPublisher : IHealthEventObservable, IHealthEventObserver
    {
        /// <summary>
        /// キャラクターの体力が変更された時に発行する
        /// </summary>
        public Observable<PlayerHealthChangedEvent> OnPlayerHealthChanged => onPlayerHealthChanged;
        private readonly Subject<PlayerHealthChangedEvent> onPlayerHealthChanged;

        /// <summary>
        /// キャラクターが死亡した時に発行する
        /// </summary>
        public Observable<PlayerOnDeadEvent> OnPlayerDead => onPlayerDeadSubject;
        private readonly Subject<PlayerOnDeadEvent> onPlayerDeadSubject;

        internal PlayerEventPublisher()
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
