# Quest ドメイン

## 概要

Quest ドメインは、ゲーム内のクエスト・依頼システムを管理します。\
プレイヤーが受注できる任務、進行状況の追跡、報酬管理、締切システムなどを統合的に扱います。

## ドメインモデル

### 基底クラス・識別子

#### QuestID
```csharp
public record QuestID : BasicID
{
    protected override string Prefix => "QST_";
}
```

### クエストエンティティ

#### QuestModel
```csharp
public record QuestModel
{
    public QuestID Id { get; }
    public QuestTitle Title { get; }
    public QuestDescription Description { get; }
    public PlayerID? ClientId { get; }          // 依頼人（NPCまたはプレイヤー）
    public QuestType Type { get; }
    public QuestDifficulty Difficulty { get; }
    public ImmutableArray<QuestReward> Rewards { get; }
    public ImmutableArray<QuestObjective> Objectives { get; }
    public DateTime? Deadline { get; }
    public QuestStatus Status { get; }
    public DateTime CreatedAt { get; }
    public DateTime? AcceptedAt { get; }
    public DateTime? CompletedAt { get; }
    
    public QuestModel(
        QuestID id,
        QuestTitle title,
        QuestDescription description,
        QuestType type,
        QuestDifficulty difficulty,
        ImmutableArray<QuestReward> rewards,
        ImmutableArray<QuestObjective> objectives,
        PlayerID? clientId = null,
        DateTime? deadline = null)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Type = type;
        Difficulty = difficulty;
        Rewards = rewards;
        Objectives = objectives;
        ClientId = clientId;
        Deadline = deadline;
        Status = QuestStatus.Available;
        CreatedAt = DateTime.UtcNow;
        AcceptedAt = null;
        CompletedAt = null;
    }
    
    public QuestModel Accept(PlayerID playerId)
    {
        if (Status != QuestStatus.Available)
            throw new InvalidOperationException("Quest is not available for acceptance");
        
        if (IsOverdue(DateTime.UtcNow))
            throw new InvalidOperationException("Quest deadline has passed");
        
        return this with 
        { 
            Status = QuestStatus.InProgress,
            AcceptedAt = DateTime.UtcNow
        };
    }
    
    public QuestModel Complete()
    {
        if (Status != QuestStatus.InProgress)
            throw new InvalidOperationException("Quest is not in progress");
        
        if (!AllObjectivesCompleted())
            throw new InvalidOperationException("Not all objectives are completed");
        
        return this with 
        { 
            Status = QuestStatus.Completed,
            CompletedAt = DateTime.UtcNow
        };
    }
    
    public QuestModel Abandon()
    {
        if (Status != QuestStatus.InProgress)
            throw new InvalidOperationException("Quest is not in progress");
        
        return this with { Status = QuestStatus.Available };
    }
    
    public QuestModel Fail()
    {
        if (Status != QuestStatus.InProgress)
            throw new InvalidOperationException("Quest is not in progress");
        
        return this with 
        { 
            Status = QuestStatus.Failed,
            CompletedAt = DateTime.UtcNow
        };
    }
    
    public bool IsOverdue(DateTime currentTime)
    {
        return Deadline.HasValue && 
               currentTime > Deadline.Value && 
               Status == QuestStatus.InProgress;
    }
    
    public bool AllObjectivesCompleted()
    {
        return Objectives.All(obj => obj.IsCompleted);
    }
    
    public float GetCompletionRate()
    {
        if (!Objectives.Any()) return 1.0f;
        
        var completedCount = Objectives.Count(obj => obj.IsCompleted);
        return (float)completedCount / Objectives.Length;
    }
}
```

### 値オブジェクト

#### QuestTitle & QuestDescription
```csharp
public record QuestTitle
{
    public string Value { get; }
    
    public QuestTitle(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Quest title cannot be empty");
        
        if (value.Length > 100)
            throw new ArgumentException("Quest title cannot exceed 100 characters");
        
        Value = value;
    }
}

public record QuestDescription
{
    public string Value { get; }
    
    public QuestDescription(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Quest description cannot be empty");
        
        if (value.Length > 1000)
            throw new ArgumentException("Quest description cannot exceed 1000 characters");
        
        Value = value;
    }
}
```

#### クエスト分類
```csharp
public enum QuestStatus
{
    Available = 1,   // 受注可能
    InProgress = 2,  // 進行中
    Completed = 3,   // 完了
    Failed = 4,      // 失敗
    Expired = 5      // 期限切れ
}

public enum QuestType
{
    Main = 1,        // メインクエスト
    Side = 2,        // サイドクエスト
    Daily = 3,       // 日次クエスト
    Weekly = 4,      // 週次クエスト
    Event = 5,       // イベントクエスト
    Guild = 6        // ギルドクエスト
}

public enum QuestDifficulty
{
    Easy = 1,        // 簡単
    Normal = 2,      // 普通
    Hard = 3,        // 困難
    Expert = 4,      // 専門家
    Master = 5       // 達人級
}
```

### クエスト目標

#### QuestObjective
```csharp
public record QuestObjective
{
    public ObjectiveID Id { get; }
    public ObjectiveDescription Description { get; }
    public ObjectiveType Type { get; }
    public int CurrentProgress { get; }
    public int RequiredProgress { get; }
    public bool IsCompleted => CurrentProgress >= RequiredProgress;
    
    public QuestObjective(
        ObjectiveID id,
        ObjectiveDescription description,
        ObjectiveType type,
        int requiredProgress,
        int currentProgress = 0)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Type = type;
        RequiredProgress = Math.Max(1, requiredProgress);
        CurrentProgress = Math.Max(0, currentProgress);
    }
    
    public QuestObjective UpdateProgress(int progress)
    {
        if (progress < 0)
            throw new ArgumentException("Progress cannot be negative");
        
        return this with { CurrentProgress = Math.Min(RequiredProgress, progress) };
    }
    
    public QuestObjective AddProgress(int additionalProgress)
    {
        if (additionalProgress < 0)
            throw new ArgumentException("Additional progress cannot be negative");
        
        return UpdateProgress(CurrentProgress + additionalProgress);
    }
    
    public float GetProgressPercentage()
    {
        return RequiredProgress > 0 ? (float)CurrentProgress / RequiredProgress : 1.0f;
    }
}

public record ObjectiveID : BasicID
{
    protected override string Prefix => "OBJ_";
}

public record ObjectiveDescription
{
    public string Value { get; }
    
    public ObjectiveDescription(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Objective description cannot be empty");
        
        Value = value;
    }
}

public enum ObjectiveType
{
    KillEnemies = 1,     // 敵討伐
    CollectItems = 2,    // アイテム収集
    CraftItems = 3,      // アイテム作成
    ReachLocation = 4,   // 場所到達
    TalkToNPC = 5,       // NPC会話
    DeliverItems = 6,    // アイテム配達
    AchieveLevel = 7,    // レベル達成
    LearnSkill = 8       // スキル習得
}
```

### 報酬システム

#### QuestReward
```csharp
public record QuestReward
{
    public RewardType Type { get; }
    public IItem? Item { get; }
    public Currency? Currency { get; }
    public Experience? Experience { get; }
    public int Quantity { get; }
    
    private QuestReward(RewardType type, int quantity, IItem? item = null, Currency? currency = null, Experience? experience = null)
    {
        Type = type;
        Quantity = Math.Max(1, quantity);
        Item = item;
        Currency = currency;
        Experience = experience;
    }
    
    public static QuestReward CreateItemReward(IItem item, int quantity)
    {
        return new QuestReward(RewardType.Item, quantity, item: item);
    }
    
    public static QuestReward CreateCurrencyReward(Currency currency)
    {
        return new QuestReward(RewardType.Currency, currency.Value, currency: currency);
    }
    
    public static QuestReward CreateExperienceReward(Experience experience)
    {
        return new QuestReward(RewardType.Experience, experience.Value, experience: experience);
    }
}

public enum RewardType
{
    Item = 1,        // アイテム報酬
    Currency = 2,    // 通貨報酬
    Experience = 3   // 経験値報酬
}
```

## ビジネスルール

### クエスト状態遷移

#### 状態遷移制御
```csharp
public static class QuestStatusTransition
{
    public static bool CanTransitionTo(QuestStatus from, QuestStatus to)
    {
        return (from, to) switch
        {
            (QuestStatus.Available, QuestStatus.InProgress) => true,
            (QuestStatus.InProgress, QuestStatus.Completed) => true,
            (QuestStatus.InProgress, QuestStatus.Failed) => true,
            (QuestStatus.InProgress, QuestStatus.Available) => true, // 放棄
            (QuestStatus.InProgress, QuestStatus.Expired) => true,   // 期限切れ
            _ => false
        };
    }
    
    public static QuestModel ProcessDeadlineCheck(QuestModel quest, DateTime currentTime)
    {
        if (quest.IsOverdue(currentTime) && quest.Status == QuestStatus.InProgress)
        {
            return quest with { Status = QuestStatus.Expired };
        }
        
        return quest;
    }
}
```

### 目標進行制限

#### 進行条件チェック
```csharp
public static class ObjectiveProgressValidator
{
    public static bool CanUpdateObjective(QuestModel quest, ObjectiveID objectiveId)
    {
        if (quest.Status != QuestStatus.InProgress)
            return false;
        
        var objective = quest.Objectives.FirstOrDefault(o => o.Id == objectiveId);
        return objective != null && !objective.IsCompleted;
    }
    
    public static QuestModel UpdateObjectiveProgress(
        QuestModel quest,
        ObjectiveID objectiveId,
        int progress)
    {
        if (!CanUpdateObjective(quest, objectiveId))
            throw new InvalidOperationException("Cannot update objective progress");
        
        var objectiveIndex = quest.Objectives.ToList().FindIndex(o => o.Id == objectiveId);
        if (objectiveIndex < 0)
            throw new ArgumentException("Objective not found");
        
        var updatedObjective = quest.Objectives[objectiveIndex].UpdateProgress(progress);
        var updatedObjectives = quest.Objectives.SetItem(objectiveIndex, updatedObjective);
        
        return quest with { Objectives = updatedObjectives };
    }
}
```

### 報酬制限

#### 報酬受取制限
```csharp
public static class QuestRewardValidator
{
    public static bool CanReceiveRewards(QuestModel quest)
    {
        return quest.Status == QuestStatus.Completed;
    }
    
    public static void ValidateRewardClaim(QuestModel quest, PlayerCommonEntity player)
    {
        if (!CanReceiveRewards(quest))
            throw new InvalidOperationException("Quest must be completed to receive rewards");
        
        // インベントリ容量チェック（アイテム報酬）
        var itemRewards = quest.Rewards.Where(r => r.Type == RewardType.Item);
        foreach (var reward in itemRewards)
        {
            // 実装は Inventory ドメインと連携
        }
    }
}
```

## ゲームロジック

### クエスト作成

```csharp
public static class QuestFactory
{
    public static QuestModel CreateBasicQuest(
        string title,
        string description,
        QuestType type,
        QuestDifficulty difficulty,
        IEnumerable<QuestObjective> objectives,
        IEnumerable<QuestReward> rewards,
        TimeSpan? duration = null)
    {
        var questId = new QuestID();
        var questTitle = new QuestTitle(title);
        var questDescription = new QuestDescription(description);
        
        var deadline = duration.HasValue ? DateTime.UtcNow.Add(duration.Value) : null;
        
        return new QuestModel(
            questId,
            questTitle,
            questDescription,
            type,
            difficulty,
            rewards.ToImmutableArray(),
            objectives.ToImmutableArray(),
            deadline: deadline
        );
    }
    
    public static QuestObjective CreateKillObjective(string enemyName, int killCount)
    {
        return new QuestObjective(
            new ObjectiveID(),
            new ObjectiveDescription($"Defeat {killCount} {enemyName}"),
            ObjectiveType.KillEnemies,
            killCount
        );
    }
    
    public static QuestObjective CreateCollectionObjective(IItem item, int quantity)
    {
        return new QuestObjective(
            new ObjectiveID(),
            new ObjectiveDescription($"Collect {quantity} {item.ItemName}"),
            ObjectiveType.CollectItems,
            quantity
        );
    }
    
    public static QuestObjective CreateLocationObjective(FieldID targetField)
    {
        return new QuestObjective(
            new ObjectiveID(),
            new ObjectiveDescription($"Reach the specified location"),
            ObjectiveType.ReachLocation,
            1
        );
    }
}
```

### クエスト管理

```csharp
public static class QuestManager
{
    public static IEnumerable<QuestModel> GetAvailableQuests(
        IEnumerable<QuestModel> allQuests,
        PlayerCommonEntity player)
    {
        return allQuests.Where(quest => 
            quest.Status == QuestStatus.Available &&
            MeetsRequirements(quest, player));
    }
    
    public static IEnumerable<QuestModel> GetActiveQuests(
        IEnumerable<QuestModel> allQuests,
        PlayerID playerId)
    {
        return allQuests.Where(quest => 
            quest.Status == QuestStatus.InProgress);
    }
    
    public static IEnumerable<QuestModel> GetCompletableQuests(
        IEnumerable<QuestModel> allQuests,
        PlayerID playerId)
    {
        return allQuests.Where(quest => 
            quest.Status == QuestStatus.InProgress &&
            quest.AllObjectivesCompleted());
    }
    
    private static bool MeetsRequirements(QuestModel quest, PlayerCommonEntity player)
    {
        // レベル要件チェック（将来実装）
        var requiredLevel = GetRequiredLevel(quest.Difficulty);
        if (player.Level.Value < requiredLevel)
            return false;
        
        // 前提クエスト要件チェック（将来実装）
        
        return true;
    }
    
    private static int GetRequiredLevel(QuestDifficulty difficulty)
    {
        return difficulty switch
        {
            QuestDifficulty.Easy => 1,
            QuestDifficulty.Normal => 5,
            QuestDifficulty.Hard => 15,
            QuestDifficulty.Expert => 30,
            QuestDifficulty.Master => 50,
            _ => 1
        };
    }
}
```

### 進行追跡

```csharp
public static class QuestProgressTracker
{
    public static QuestModel TrackKillProgress(
        QuestModel quest,
        string enemyName,
        int killCount = 1)
    {
        var killObjectives = quest.Objectives.Where(obj => 
            obj.Type == ObjectiveType.KillEnemies &&
            obj.Description.Value.Contains(enemyName));
        
        var updatedQuest = quest;
        foreach (var objective in killObjectives)
        {
            updatedQuest = ObjectiveProgressValidator.UpdateObjectiveProgress(
                updatedQuest, objective.Id, objective.CurrentProgress + killCount);
        }
        
        return updatedQuest;
    }
    
    public static QuestModel TrackItemCollection(
        QuestModel quest,
        IItem item,
        int quantity)
    {
        var collectionObjectives = quest.Objectives.Where(obj => 
            obj.Type == ObjectiveType.CollectItems &&
            obj.Description.Value.Contains(item.ItemName));
        
        var updatedQuest = quest;
        foreach (var objective in collectionObjectives)
        {
            updatedQuest = ObjectiveProgressValidator.UpdateObjectiveProgress(
                updatedQuest, objective.Id, objective.CurrentProgress + quantity);
        }
        
        return updatedQuest;
    }
    
    public static QuestModel TrackLocationReach(
        QuestModel quest,
        FieldID currentField)
    {
        var locationObjectives = quest.Objectives.Where(obj => 
            obj.Type == ObjectiveType.ReachLocation);
        
        var updatedQuest = quest;
        foreach (var objective in locationObjectives)
        {
            // 位置チェック（将来実装：目標位置との照合）
            updatedQuest = ObjectiveProgressValidator.UpdateObjectiveProgress(
                updatedQuest, objective.Id, 1);
        }
        
        return updatedQuest;
    }
}
```

### 報酬処理

```csharp
public static class QuestRewardProcessor
{
    public static (PlayerCommonEntity updatedPlayer, InfiniteSlotInventory updatedInventory, Wallet updatedWallet) 
        ProcessRewards(
            QuestModel quest,
            PlayerCommonEntity player,
            InfiniteSlotInventory inventory,
            Wallet wallet)
    {
        QuestRewardValidator.ValidateRewardClaim(quest, player);
        
        var updatedPlayer = player;
        var updatedInventory = inventory;
        var updatedWallet = wallet;
        
        foreach (var reward in quest.Rewards)
        {
            switch (reward.Type)
            {
                case RewardType.Experience:
                    if (reward.Experience != null)
                    {
                        var newExp = new Experience(player.Experience.Value + reward.Experience.Value);
                        updatedPlayer = updatedPlayer with { Experience = newExp };
                    }
                    break;
                
                case RewardType.Currency:
                    if (reward.Currency != null)
                    {
                        updatedWallet = updatedWallet.AddCurrency(reward.Currency);
                    }
                    break;
                
                case RewardType.Item:
                    if (reward.Item != null)
                    {
                        updatedInventory = updatedInventory.AddItem(reward.Item, reward.Quantity);
                    }
                    break;
            }
        }
        
        return (updatedPlayer, updatedInventory, updatedWallet);
    }
}
```

## 他ドメインとの連携

### Character ドメインとの連携
- **進行条件**: レベル・ステータス要件
- **経験値報酬**: クエスト完了による経験値獲得
- 詳細: [Character.md](./Character.md)

### Item ドメインとの連携
- **収集目標**: 特定アイテムの収集クエスト
- **アイテム報酬**: クエスト報酬としてのアイテム付与
- 詳細: [Item.md](./Item.md)

### Inventory ドメインとの連携
- **報酬受取**: インベントリ容量チェック
- **アイテム管理**: 報酬アイテムの格納
- 詳細: [Inventory.md](./Inventory.md)

### Field ドメインとの連携
- **位置目標**: 特定エリアへの到達クエスト
- **エリア限定**: 特定エリアでのみ進行可能なクエスト
- 詳細: [Field.md](./Field.md)

### Skill ドメインとの連携
- **スキル目標**: 特定スキル習得クエスト
- **制作目標**: 生産スキルによるアイテム作成クエスト
- 詳細: [Skill.md](./Skill.md)

## 拡張ポイント

### クエストチェーンシステム
```csharp
// 連続クエストシステム
public record QuestChain
{
    public QuestChainID Id { get; }
    public string Name { get; }
    public ImmutableArray<QuestID> QuestSequence { get; }
    public int CurrentQuestIndex { get; }
    
    public QuestID? GetNextQuest() => 
        CurrentQuestIndex < QuestSequence.Length - 1 
            ? QuestSequence[CurrentQuestIndex + 1] 
            : null;
}
```

### 動的クエスト生成
```csharp
// 手続き生成クエスト
public interface IQuestGenerator
{
    QuestModel GenerateQuest(PlayerCommonEntity player, QuestType type);
}

public class RandomKillQuestGenerator : IQuestGenerator
{
    public QuestModel GenerateQuest(PlayerCommonEntity player, QuestType type)
    {
        // プレイヤーレベルに応じた敵討伐クエストを生成
        return QuestFactory.CreateBasicQuest(/* ... */);
    }
}
```

### 共有クエストシステム
```csharp
// プレイヤー間共有クエスト
public record SharedQuest : QuestModel
{
    public ImmutableArray<PlayerID> Participants { get; }
    public int MaxParticipants { get; }
    
    public SharedQuest AddParticipant(PlayerID playerId) => 
        Participants.Length < MaxParticipants 
            ? this with { Participants = Participants.Add(playerId) }
            : throw new InvalidOperationException("Quest is full");
}
```

### 期間限定クエストシステム
```csharp
// イベント・期間限定クエスト
public record TimeLimitedQuest : QuestModel
{
    public DateTime StartTime { get; }
    public DateTime EndTime { get; }
    
    public bool IsActive(DateTime currentTime) => 
        currentTime >= StartTime && currentTime <= EndTime;
}
```

Quest ドメインは、プレイヤーの目標設定とゲーム進行の動機づけを提供する重要なシステムです。\
他のドメインと密接に連携し、統合的なゲーム体験を実現します。