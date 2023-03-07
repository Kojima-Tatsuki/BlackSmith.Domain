using System;
using System.Collections.Generic;
using UniRx;

namespace BlackSmith.Domain.Player.Event
{
    /// <summary>
    /// プレイヤーエンティティのイベントを発行する
    /// </summary>
    public class PlayerEventPublisher
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

        internal void SetChangedPlayerHealth(PlayerHealthChangedEvent changedEvent)
        {
            onPlayerHealthChanged.OnNext(changedEvent);
        }

        internal void SetOnPlayerDead(PlayerOnDeadEvent deadEvent)
        {
            onPlayerDeadSubject.OnNext(deadEvent);
        }
    }
}
