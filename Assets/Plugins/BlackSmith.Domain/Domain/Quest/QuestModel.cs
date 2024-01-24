using BlackSmith.Domain.Character;

namespace BlackSmith.Domain.Quest
{
    class QuestModel
    {
        public string QuestName { get; init; }

        public string Discription { get; init; }

        // 依頼の締め切り、有効期限

        // 依頼の報酬

        // 依頼人
        CharacterID ClientId { get; init; }

        internal QuestModel(string questName, string discription, CharacterID clientId)
        {
            QuestName = questName;
            Discription = discription;
            ClientId = clientId;
        }
    }
}
