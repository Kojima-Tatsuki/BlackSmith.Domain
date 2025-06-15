# システム統合パターン

## 概要

BlackSmith.Domain における複数ドメインを横断するシステム統合パターンを説明します。複合システム間の連携、データフロー管理、エラーハンドリング、パフォーマンス最適化の戦略を詳述します。

## 統合アーキテクチャパターン

### 1. ファサードパターン（Facade Pattern）

複数ドメインの複雑な相互作用を単一のインターフェースで隠蔽します。

```csharp
public class GameplayFacade
{
    private readonly BattleSystem battleSystem;
    private readonly CraftingSystem craftingSystem;
    private readonly EquipmentSystem equipmentSystem;
    private readonly LevelingSystem levelingSystem;
    
    public GameplayFacade(
        BattleSystem battleSystem,
        CraftingSystem craftingSystem,
        EquipmentSystem equipmentSystem,
        LevelingSystem levelingSystem)
    {
        this.battleSystem = battleSystem;
        this.craftingSystem = craftingSystem;
        this.equipmentSystem = equipmentSystem;
        this.levelingSystem = levelingSystem;
    }
    
    // 戦闘からレベリングまでの一連の流れ
    public async Task<CombatSessionResult> ExecuteCombatSessionAsync(
        PlayerCommonEntity player,
        EquipmentInventory equipment,
        InfiniteSlotInventory inventory,
        EffectCollection effects,
        EnemyParameters enemy)
    {
        // 1. 戦闘システム初期化
        var battleSequence = battleSystem.StartBattle(player, equipment, effects);
        
        // 2. 戦闘実行
        var combatResult = await battleSystem.ExecuteCombatAsync(battleSequence, enemy);
        
        // 3. 経験値・レベリング処理
        var levelingResult = await levelingSystem.ProcessCombatExperienceAsync(
            combatResult.UpdatedPlayer,
            combatResult.UpdatedEffects,
            combatResult.ExperienceGained,
            ExperienceSource.Combat
        );
        
        // 4. 装備耐久度処理（将来実装）
        var equipmentResult = await equipmentSystem.ProcessCombatWearAsync(
            levelingResult.UpdatedPlayer,
            equipment
        );
        
        return new CombatSessionResult(
            levelingResult.UpdatedPlayer,
            equipmentResult.UpdatedEquipment,
            inventory,
            levelingResult.UpdatedEffects,
            combatResult.IsVictory,
            levelingResult.LevelUpsGained,
            combatResult.ExperienceGained
        );
    }
    
    // クラフトから装備まで統合フロー
    public async Task<CraftingSessionResult> ExecuteCraftingSessionAsync(
        PlayerCommonEntity player,
        InfiniteSlotInventory inventory,
        ICraftableItem targetItem,
        bool autoEquipIfBetter = false)
    {
        // 1. クラフト実行
        var craftingResult = await craftingSystem.ExecuteCraftingAsync(
            player, inventory, targetItem);
        
        if (!craftingResult.IsSuccess)
            return CraftingSessionResult.Failed(craftingResult.Message);
        
        // 2. 自動装備チェック
        if (autoEquipIfBetter && 
            craftingResult.CreatedItems.Any(item => item.Item is EquippableItem))
        {
            var newEquipment = craftingResult.CreatedItems
                .First(item => item.Item is EquippableItem).Item as EquippableItem;
            
            var equipmentResult = await equipmentSystem.TryAutoEquipAsync(
                craftingResult.UpdatedPlayer,
                newEquipment!,
                craftingResult.UpdatedInventory
            );
            
            return new CraftingSessionResult(
                equipmentResult.UpdatedPlayer ?? craftingResult.UpdatedPlayer,
                equipmentResult.UpdatedInventory ?? craftingResult.UpdatedInventory,
                equipmentResult.UpdatedEquipment,
                craftingResult.CreatedItems,
                equipmentResult.IsSuccess ? "Crafted and equipped!" : craftingResult.Message
            );
        }
        
        return new CraftingSessionResult(
            craftingResult.UpdatedPlayer,
            craftingResult.UpdatedInventory,
            null,
            craftingResult.CreatedItems,
            craftingResult.Message
        );
    }
}
```

### 2. メディエーターパターン（Mediator Pattern）

ドメイン間の通信を中央集権的に管理します。

```csharp
public interface IDomainMediator
{
    Task<TResponse> SendAsync<TResponse>(IDomainRequest<TResponse> request);
    Task PublishAsync(IDomainEvent domainEvent);
}

public class DomainMediator : IDomainMediator
{
    private readonly IServiceProvider serviceProvider;
    
    public DomainMediator(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }
    
    public async Task<TResponse> SendAsync<TResponse>(IDomainRequest<TResponse> request)
    {
        var handlerType = typeof(IDomainRequestHandler<,>)
            .MakeGenericType(request.GetType(), typeof(TResponse));
        
        var handler = serviceProvider.GetService(handlerType);
        
        if (handler == null)
            throw new InvalidOperationException($"No handler found for {request.GetType().Name}");
        
        var method = handlerType.GetMethod("HandleAsync");
        var result = await (Task<TResponse>)method!.Invoke(handler, new[] { request });
        
        return result;
    }
    
    public async Task PublishAsync(IDomainEvent domainEvent)
    {
        var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
        var handlers = serviceProvider.GetServices(handlerType);
        
        var tasks = handlers.Select(handler =>
        {
            var method = handlerType.GetMethod("HandleAsync");
            return (Task)method!.Invoke(handler, new[] { domainEvent });
        });
        
        await Task.WhenAll(tasks);
    }
}

// リクエスト例: 装備変更要求
public record ChangeEquipmentRequest(
    PlayerID PlayerId,
    EquippableItem NewEquipment) : IDomainRequest<EquipmentChangeResult>;

public class ChangeEquipmentHandler : IDomainRequestHandler<ChangeEquipmentRequest, EquipmentChangeResult>
{
    private readonly EquipmentIntegrationService equipmentService;
    private readonly IDomainMediator mediator;
    
    public async Task<EquipmentChangeResult> HandleAsync(ChangeEquipmentRequest request)
    {
        // 装備変更実行
        var result = await equipmentService.ChangeEquipmentAsync(
            request.PlayerId, request.NewEquipment);
        
        if (result.IsSuccess)
        {
            // イベント発行
            await mediator.PublishAsync(new EquipmentChangedEvent(
                request.PlayerId, request.NewEquipment));
        }
        
        return result;
    }
}

// イベント例: 装備変更イベント
public record EquipmentChangedEvent(
    PlayerID PlayerId,
    EquippableItem NewEquipment) : IDomainEvent;

public class EquipmentChangedEventHandler : IDomainEventHandler<EquipmentChangedEvent>
{
    private readonly BattleSystem battleSystem;
    private readonly QuestProgressService questService;
    
    public async Task HandleAsync(EquipmentChangedEvent domainEvent)
    {
        // 戦闘パラメータ再計算
        await battleSystem.RecalculateParametersAsync(domainEvent.PlayerId);
        
        // クエスト進行チェック（装備関連目標）
        await questService.CheckEquipmentObjectivesAsync(
            domainEvent.PlayerId, domainEvent.NewEquipment);
    }
}
```

### 3. Sagaパターン（分散トランザクション）

複数ドメインにまたがる複雑な業務プロセスを管理します。

```csharp
public abstract class DomainSaga
{
    protected readonly List<SagaStep> steps = new();
    protected readonly List<SagaStep> compensations = new();
    
    public async Task<SagaResult> ExecuteAsync()
    {
        var executedSteps = new List<SagaStep>();
        
        try
        {
            foreach (var step in steps)
            {
                await step.ExecuteAsync();
                executedSteps.Add(step);
            }
            
            return SagaResult.Success();
        }
        catch (Exception ex)
        {
            // 補償トランザクション実行
            await CompensateAsync(executedSteps);
            return SagaResult.Failed(ex.Message);
        }
    }
    
    private async Task CompensateAsync(List<SagaStep> executedSteps)
    {
        // 逆順で補償実行
        executedSteps.Reverse();
        
        foreach (var step in executedSteps)
        {
            try
            {
                await step.CompensateAsync();
            }
            catch (Exception ex)
            {
                // 補償失敗はログのみ（手動対応が必要）
                Console.WriteLine($"Compensation failed for step {step.Name}: {ex.Message}");
            }
        }
    }
}

// 例: アイテム購入Saga
public class ItemPurchaseSaga : DomainSaga
{
    private readonly PlayerCommonEntity player;
    private readonly IItem purchaseItem;
    private readonly Currency price;
    
    public ItemPurchaseSaga(PlayerCommonEntity player, IItem item, Currency price)
    {
        this.player = player;
        this.purchaseItem = item;
        this.price = price;
        
        SetupSteps();
    }
    
    private void SetupSteps()
    {
        // Step 1: 通貨減算
        steps.Add(new DeductCurrencyStep(player.Id, price));
        
        // Step 2: アイテム追加
        steps.Add(new AddItemToInventoryStep(player.Id, purchaseItem, 1));
        
        // Step 3: 購入履歴記録
        steps.Add(new RecordPurchaseHistoryStep(player.Id, purchaseItem, price));
        
        // Step 4: クエスト進行チェック
        steps.Add(new CheckQuestProgressStep(player.Id, purchaseItem));
    }
}

public class DeductCurrencyStep : SagaStep
{
    private readonly PlayerID playerId;
    private readonly Currency amount;
    
    public override async Task ExecuteAsync()
    {
        // Inventory ドメイン: 通貨減算
        await walletService.DeductCurrencyAsync(playerId, amount);
    }
    
    public override async Task CompensateAsync()
    {
        // 補償: 通貨返却
        await walletService.AddCurrencyAsync(playerId, amount);
    }
}
```

## イベント駆動アーキテクチャ

### 1. ドメインイベント設計

```csharp
public interface IDomainEvent
{
    Guid EventId { get; }
    DateTime OccurredAt { get; }
    string EventType { get; }
}

public abstract record DomainEvent : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
    public virtual string EventType => GetType().Name;
}

// 具体的なイベント定義
public record PlayerLevelUpEvent(
    PlayerID PlayerId, 
    int OldLevel, 
    int NewLevel, 
    int StatusPointsGained) : DomainEvent;

public record ItemCraftedEvent(
    PlayerID PlayerId,
    ICraftableItem CraftedItem,
    bool WasSuccessful,
    int ExperienceGained) : DomainEvent;

public record QuestCompletedEvent(
    PlayerID PlayerId,
    QuestID QuestId,
    ImmutableArray<QuestReward> Rewards) : DomainEvent;
```

### 2. イベントハンドラー

```csharp
// レベルアップイベントの複数ハンドラー
public class LevelUpQuestProgressHandler : IDomainEventHandler<PlayerLevelUpEvent>
{
    private readonly QuestProgressService questService;
    
    public async Task HandleAsync(PlayerLevelUpEvent evt)
    {
        // レベル到達目標のあるクエストの進行更新
        await questService.UpdateLevelObjectivesAsync(evt.PlayerId, evt.NewLevel);
    }
}

public class LevelUpSkillUnlockHandler : IDomainEventHandler<PlayerLevelUpEvent>
{
    private readonly SkillUnlockService skillService;
    
    public async Task HandleAsync(PlayerLevelUpEvent evt)
    {
        // 新レベルで習得可能になったスキルの通知
        await skillService.CheckNewSkillUnlocksAsync(evt.PlayerId, evt.NewLevel);
    }
}

public class LevelUpEffectApplicationHandler : IDomainEventHandler<PlayerLevelUpEvent>
{
    private readonly PassiveEffectService effectService;
    
    public async Task HandleAsync(PlayerLevelUpEvent evt)
    {
        // レベルアップボーナス効果の適用
        var levelUpBuff = EffectFactory.CreateExperienceBoost(1.2f, 10);
        await effectService.ApplyEffectAsync(evt.PlayerId, levelUpBuff);
    }
}
```

### 3. イベントストア（将来拡張）

```csharp
public interface IEventStore
{
    Task SaveEventAsync(IDomainEvent domainEvent);
    Task<IEnumerable<IDomainEvent>> GetEventsAsync(Guid aggregateId);
    Task<IEnumerable<IDomainEvent>> GetEventsByTypeAsync(string eventType);
}

public class InMemoryEventStore : IEventStore
{
    private readonly ConcurrentDictionary<Guid, List<IDomainEvent>> eventsByAggregate = new();
    private readonly ConcurrentDictionary<string, List<IDomainEvent>> eventsByType = new();
    
    public async Task SaveEventAsync(IDomainEvent domainEvent)
    {
        // 集約別保存
        var aggregateId = ExtractAggregateId(domainEvent);
        eventsByAggregate.AddOrUpdate(
            aggregateId,
            [domainEvent],
            (key, existing) => existing.Concat([domainEvent]).ToList()
        );
        
        // 型別保存
        eventsByType.AddOrUpdate(
            domainEvent.EventType,
            [domainEvent],
            (key, existing) => existing.Concat([domainEvent]).ToList()
        );
        
        await Task.CompletedTask;
    }
    
    private Guid ExtractAggregateId(IDomainEvent domainEvent)
    {
        // リフレクションまたはパターンマッチングで集約IDを抽出
        return domainEvent switch
        {
            PlayerLevelUpEvent evt => Guid.Parse(evt.PlayerId.Value.Split('_')[1]),
            ItemCraftedEvent evt => Guid.Parse(evt.PlayerId.Value.Split('_')[1]),
            _ => Guid.Empty
        };
    }
}
```

## エラーハンドリング統合

### 1. 階層化エラーハンドリング

```csharp
public abstract class DomainException : Exception
{
    public string DomainName { get; }
    public string ErrorCode { get; }
    
    protected DomainException(string domainName, string errorCode, string message) 
        : base(message)
    {
        DomainName = domainName;
        ErrorCode = errorCode;
    }
}

// ドメイン固有例外
public class CharacterDomainException : DomainException
{
    public CharacterDomainException(string errorCode, string message)
        : base("Character", errorCode, message) { }
    
    public static CharacterDomainException InsufficientLevel(int required, int current) =>
        new("INSUFFICIENT_LEVEL", $"Required level {required}, current {current}");
    
    public static CharacterDomainException InsufficientStatusPoints(int required, int available) =>
        new("INSUFFICIENT_STATUS_POINTS", $"Required {required} points, available {available}");
}

public class ItemDomainException : DomainException
{
    public ItemDomainException(string errorCode, string message)
        : base("Item", errorCode, message) { }
    
    public static ItemDomainException InvalidEnhancement(string reason) =>
        new("INVALID_ENHANCEMENT", $"Enhancement failed: {reason}");
}

// 統合エラーハンドラー
public class DomainErrorHandler
{
    public DomainOperationResult HandleError(Exception exception)
    {
        return exception switch
        {
            CharacterDomainException charEx => new DomainOperationResult(
                false, 
                charEx.Message, 
                charEx.ErrorCode,
                charEx.DomainName
            ),
            
            ItemDomainException itemEx => new DomainOperationResult(
                false,
                itemEx.Message,
                itemEx.ErrorCode,
                itemEx.DomainName
            ),
            
            AggregateException aggEx when aggEx.InnerExceptions.All(ex => ex is DomainException) =>
                HandleMultipleDomainErrors(aggEx.InnerExceptions.Cast<DomainException>()),
            
            _ => new DomainOperationResult(
                false,
                "An unexpected error occurred",
                "UNKNOWN_ERROR",
                "System"
            )
        };
    }
    
    private DomainOperationResult HandleMultipleDomainErrors(IEnumerable<DomainException> exceptions)
    {
        var errors = exceptions.Select(ex => $"{ex.DomainName}: {ex.Message}");
        return new DomainOperationResult(
            false,
            string.Join("; ", errors),
            "MULTIPLE_DOMAIN_ERRORS",
            "System"
        );
    }
}
```

### 2. Circuit Breaker パターン

```csharp
public class DomainCircuitBreaker
{
    private readonly string serviceName;
    private readonly int failureThreshold;
    private readonly TimeSpan timeout;
    private int failureCount;
    private DateTime lastFailureTime;
    private CircuitBreakerState state = CircuitBreakerState.Closed;
    
    public async Task<T> ExecuteAsync<T>(Func<Task<T>> operation)
    {
        if (state == CircuitBreakerState.Open)
        {
            if (DateTime.UtcNow - lastFailureTime > timeout)
            {
                state = CircuitBreakerState.HalfOpen;
            }
            else
            {
                throw new CircuitBreakerOpenException(serviceName);
            }
        }
        
        try
        {
            var result = await operation();
            
            if (state == CircuitBreakerState.HalfOpen)
            {
                state = CircuitBreakerState.Closed;
                failureCount = 0;
            }
            
            return result;
        }
        catch (Exception)
        {
            failureCount++;
            lastFailureTime = DateTime.UtcNow;
            
            if (failureCount >= failureThreshold)
            {
                state = CircuitBreakerState.Open;
            }
            
            throw;
        }
    }
}

public enum CircuitBreakerState
{
    Closed,   // 正常動作
    Open,     // 障害発生中
    HalfOpen  // 復旧テスト中
}
```

## パフォーマンス最適化

### 1. キャッシュ統合戦略

```csharp
public class DomainCacheManager
{
    private readonly IMemoryCache memoryCache;
    private readonly IDistributedCache distributedCache;
    
    public async Task<T> GetOrSetAsync<T>(
        string key,
        Func<Task<T>> factory,
        TimeSpan? expiration = null,
        CacheLevel level = CacheLevel.Memory)
    {
        var cacheKey = $"domain:{typeof(T).Name}:{key}";
        
        // メモリキャッシュ確認
        if (memoryCache.TryGetValue(cacheKey, out T cachedValue))
        {
            return cachedValue;
        }
        
        // 分散キャッシュ確認（指定時のみ）
        if (level == CacheLevel.Distributed)
        {
            var distributedValue = await GetFromDistributedCacheAsync<T>(cacheKey);
            if (distributedValue != null)
            {
                memoryCache.Set(cacheKey, distributedValue, TimeSpan.FromMinutes(5));
                return distributedValue;
            }
        }
        
        // ファクトリーから取得
        var value = await factory();
        
        // キャッシュ保存
        var expirationTime = expiration ?? TimeSpan.FromMinutes(30);
        memoryCache.Set(cacheKey, value, expirationTime);
        
        if (level == CacheLevel.Distributed)
        {
            await SetToDistributedCacheAsync(cacheKey, value, expirationTime);
        }
        
        return value;
    }
    
    public async Task InvalidateAsync(string pattern)
    {
        // パターンマッチによるキャッシュ無効化
        await InvalidateMemoryCacheAsync(pattern);
        await InvalidateDistributedCacheAsync(pattern);
    }
}

public enum CacheLevel
{
    Memory,      // メモリキャッシュのみ
    Distributed  // 分散キャッシュまで使用
}

// キャッシュ適用例
public class CachedPlayerService
{
    private readonly DomainCacheManager cacheManager;
    private readonly IPlayerRepository playerRepository;
    
    public async Task<PlayerCommonEntity> GetPlayerAsync(PlayerID playerId)
    {
        return await cacheManager.GetOrSetAsync(
            playerId.Value,
            () => playerRepository.FindByIdAsync(playerId),
            TimeSpan.FromMinutes(15),
            CacheLevel.Distributed
        );
    }
    
    public async Task UpdatePlayerAsync(PlayerCommonEntity player)
    {
        await playerRepository.SaveAsync(player);
        
        // キャッシュ無効化
        await cacheManager.InvalidateAsync($"*{player.Id.Value}*");
    }
}
```

### 2. バッチ処理最適化

```csharp
public class DomainBatchProcessor
{
    private readonly ConcurrentQueue<IDomainOperation> operationQueue = new();
    private readonly Timer batchTimer;
    private readonly int maxBatchSize;
    
    public DomainBatchProcessor(int maxBatchSize = 100, TimeSpan? batchInterval = null)
    {
        this.maxBatchSize = maxBatchSize;
        var interval = batchInterval ?? TimeSpan.FromSeconds(5);
        
        batchTimer = new Timer(ProcessBatch, null, interval, interval);
    }
    
    public Task EnqueueAsync(IDomainOperation operation)
    {
        operationQueue.Enqueue(operation);
        
        if (operationQueue.Count >= maxBatchSize)
        {
            _ = Task.Run(() => ProcessBatch(null));
        }
        
        return Task.CompletedTask;
    }
    
    private async void ProcessBatch(object? state)
    {
        var batch = new List<IDomainOperation>();
        
        // キューから操作を取得
        while (batch.Count < maxBatchSize && operationQueue.TryDequeue(out var operation))
        {
            batch.Add(operation);
        }
        
        if (!batch.Any()) return;
        
        // 種別ごとにグループ化して効率的に処理
        var groupedOperations = batch.GroupBy(op => op.GetType());
        
        var tasks = groupedOperations.Select(async group =>
        {
            try
            {
                await ProcessOperationGroupAsync(group);
            }
            catch (Exception ex)
            {
                // バッチ内エラーはログのみ
                Console.WriteLine($"Batch processing error: {ex.Message}");
            }
        });
        
        await Task.WhenAll(tasks);
    }
    
    private async Task ProcessOperationGroupAsync(IEnumerable<IDomainOperation> operations)
    {
        var operationType = operations.First().GetType();
        
        // 操作種別に応じた最適化処理
        switch (operationType.Name)
        {
            case nameof(SkillExperienceUpdateOperation):
                await ProcessSkillExperienceUpdatesAsync(operations.Cast<SkillExperienceUpdateOperation>());
                break;
                
            case nameof(QuestProgressUpdateOperation):
                await ProcessQuestProgressUpdatesAsync(operations.Cast<QuestProgressUpdateOperation>());
                break;
                
            default:
                // 通常の個別処理
                var tasks = operations.Select(op => op.ExecuteAsync());
                await Task.WhenAll(tasks);
                break;
        }
    }
}
```

このシステム統合パターンにより、BlackSmith.Domain は複雑なドメイン間連携を効率的かつ安全に管理できています。