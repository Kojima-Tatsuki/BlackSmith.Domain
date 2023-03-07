using UniRx;

namespace BlackSmith.Domain.Character.NonPlayer.Event
{
    public class NonPlayerEventPublisher
    {
        public UniRx.IObservable<NonPlayerHealthChangedEvent> OnNonPlayerHealthChanged => onHealthChanged;
        private readonly Subject<NonPlayerHealthChangedEvent> onHealthChanged;

        public NonPlayerEventPublisher()
        {
            onHealthChanged = new Subject<NonPlayerHealthChangedEvent>();
        }

        internal void SetNonPlayerHealth(NonPlayerHealthChangedEvent changedEvent)
        {
            onHealthChanged.OnNext(changedEvent);
        }
    }
}
