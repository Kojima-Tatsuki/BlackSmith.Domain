using System;
using System.Collections.Generic;
using System.Reactive;
using Microsoft.Extensions.DependencyInjection;
using BlackSmith.Domain.Player.Event;
using BlackSmith.Usecase.Interface;

namespace BlackSmith.Usecase.Player
{
    public class PlayerHealthChangedEventInstructor
    {
        private readonly PlayerEventPublisher publisher;

        private readonly IOnPlayerHealthChangedEventHundler healthChangedEventHundler;

        public PlayerHealthChangedEventInstructor()
        {
            var provider = DIContainer.Instance.ServiceProvider;

            publisher = provider.GetRequiredService<PlayerEventPublisher>();
            healthChangedEventHundler = provider.GetRequiredService<IOnPlayerHealthChangedEventHundler>();

            publisher.OnPlayerHealthChanged
                .Subscribe(changedEvent => healthChangedEventHundler
                .OnChangedHealthPoint(
                    changedEvent.HealthPoint.GetValues().current, 
                    changedEvent.HealthPoint.GetValues().max));
        }
    }
}
