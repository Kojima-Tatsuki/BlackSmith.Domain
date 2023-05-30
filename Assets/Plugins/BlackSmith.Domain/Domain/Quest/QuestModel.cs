using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackSmith.Domain.Quest
{
    class QuestModel
    {
        public string QuestName { get; init; }

        public string Discription { get; init; }

        internal QuestModel(string questName, string discription)
        {
            QuestName = questName;
            Discription = discription;
        }
    }
}
