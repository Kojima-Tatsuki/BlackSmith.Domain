﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackSmith.Domain.Character.Player.Event
{
    /// <summary>
    /// ドメインイベントの基底クラス
    /// </summary>
    public abstract class PlayerEventBase
    {
        /// <summary>
        /// イベントを発行したエンティティのID
        /// </summary>
        public CharacterID ID { get; }

        /// <summary>
        /// イベントが発行された時間
        /// </summary>
        public DateTime TimeOfEnevetPublished { get; }

        public PlayerEventBase(CharacterID id)
        {
            ID = id;

            TimeOfEnevetPublished = new DateTime();
        }
    }
}
