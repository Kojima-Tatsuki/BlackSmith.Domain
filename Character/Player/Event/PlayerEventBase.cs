using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackSmith.Domain.Player.Event
{
    /// <summary>
    /// ドメインイベントの基底クラス
    /// </summary>
    public abstract class PlayerEventBase
    {
        /// <summary>
        /// イベントを発行したエンティティのID
        /// </summary>
        public PlayerID ID { get; }

        /// <summary>
        /// イベントが発行された時間
        /// </summary>
        public DateTime TimeOfEnevetPublished { get; }

        public PlayerEventBase(PlayerID id)
        {
            ID = id;

            TimeOfEnevetPublished = new DateTime();
        }
    }
}
