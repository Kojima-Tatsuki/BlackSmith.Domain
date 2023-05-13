using UniRx;
using BlackSmith.Usecase.Interface;
using BlackSmith.Domain.Character.Player.Event;

namespace BlackSmith.Usecase.Character.Player
{
    public class PlayerHealthChangedEventInstructor
    {
        private readonly PlayerEventPublisher publisher;

        private readonly IOnPlayerHealthChangedEventHundler healthChangedEventHundler;

        public PlayerHealthChangedEventInstructor(PlayerEventPublisher playerEventPublisher, IOnPlayerHealthChangedEventHundler changedEventHundler)
        {
            publisher = playerEventPublisher;
            healthChangedEventHundler = changedEventHundler;

            publisher.OnPlayerHealthChanged
                .Subscribe(changedEvent => healthChangedEventHundler
                .OnChangedHealthPoint(
                    changedEvent.HealthPoint.GetValues().current,
                    changedEvent.HealthPoint.GetValues().max));
        }
    }
}
