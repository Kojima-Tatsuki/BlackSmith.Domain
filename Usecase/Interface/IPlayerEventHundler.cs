using System;
using System.Collections.Generic;
using BlackSmith.Usecase.Player;

namespace BlackSmith.Usecase.Interface
{
    public interface IPlayerEventHundler
    {
    }

    public interface IOnPlayerHealthChangedEventHundler : IPlayerEventHundler
    {
        void OnChangedHealthPoint(int currnethelath, int maxHealth);
    }
}
