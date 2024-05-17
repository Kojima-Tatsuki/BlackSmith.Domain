using BlackSmith.Domain.Character;

namespace BlackSmith.Domain.Quest
{
    class QuestModel
    {
        public string QuestName { get; }

        public string Discription { get; }

        // 依頼の締め切り、有効期限

        // 依頼の報酬

        // 依頼人
        CharacterID ClientId { get; }

        internal QuestModel(string questName, string discription, CharacterID clientId)
        {
            QuestName = questName;
            Discription = discription;
            ClientId = clientId;
        }
    }
}
