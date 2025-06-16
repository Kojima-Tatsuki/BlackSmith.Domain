# PassiveEffect ドメイン

## 概要

PassiveEffect ドメインは、プレイヤーに影響を与える一時的・持続的な効果を管理します。\
バフ・デバフ、ステータス効果、環境効果など、戦闘パラメータやゲームプレイに影響を与える全ての効果を統一的に扱います。

## ドメインモデル

### 基底クラス・識別子

#### EffectID
```csharp
// 【部分実装】基本的なEffectIDは実装済みだが、BasicID継承ではない
public record EffectID : BasicID
{
    protected override string Prefix => "EFF_";
}
```

### ステータス効果

#### BattleStatusEffect
```csharp
// 【部分実装】基本的なBattleStatusEffectは実装済みだが、持続時間等の機能は未実装
public record BattleStatusEffect
{
    public EffectID Id { get; }
    public EffectName Name { get; }
    public BattleStatusEffectModel StatusModel { get; }
    public int Duration { get; }
    public EffectType Type { get; }
    public bool IsStackable { get; }
    
    public BattleStatusEffect(
        EffectID id,
        EffectName name,
        BattleStatusEffectModel statusModel,
        int duration,
        EffectType type,
        bool isStackable = false)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        StatusModel = statusModel ?? throw new ArgumentNullException(nameof(statusModel));
        Duration = Math.Max(0, duration);
        Type = type;
        IsStackable = isStackable;
    }
    
    public BattleStatusEffect DecreaseDuration(int turns = 1)
    {
        return this with { Duration = Math.Max(0, Duration - turns) };
    }
    
    public bool IsExpired => Duration <= 0;
    
    public bool IsPermanent => Duration == -1; // -1は永続効果
}
```

#### BattleStatusEffectModel
```csharp
// 【部分実装】基本的なパラメータは実装済みだが、乗算系パラメータは未実装
public record BattleStatusEffectModel
{
    public int MaxHealth { get; }
    public int Attack { get; }
    public int Defense { get; }
    public int MovementSpeed { get; }
    public float AttackSpeedMultiplier { get; }
    public float ExperienceMultiplier { get; }
    
    public BattleStatusEffectModel(
        int maxHealth = 0,
        int attack = 0,
        int defense = 0,
        int movementSpeed = 0,
        float attackSpeedMultiplier = 1.0f,
        float experienceMultiplier = 1.0f)
    {
        MaxHealth = maxHealth;
        Attack = attack;
        Defense = defense;
        MovementSpeed = movementSpeed;
        AttackSpeedMultiplier = Math.Max(0.1f, attackSpeedMultiplier);
        ExperienceMultiplier = Math.Max(0.0f, experienceMultiplier);
    }
    
    // 【未実装】複数効果の統合
    public static BattleStatusEffectModel Combine(IEnumerable<BattleStatusEffectModel> effects)
    {
        if (!effects.Any())
            return new BattleStatusEffectModel();
        
        return effects.Aggregate(
            new BattleStatusEffectModel(),
            (acc, effect) => new BattleStatusEffectModel(
                acc.MaxHealth + effect.MaxHealth,
                acc.Attack + effect.Attack,
                acc.Defense + effect.Defense,
                acc.MovementSpeed + effect.MovementSpeed,
                acc.AttackSpeedMultiplier * effect.AttackSpeedMultiplier,
                acc.ExperienceMultiplier * effect.ExperienceMultiplier
            )
        );
    }
    
    // 【未実装】効果の逆転（デバフ除去用）
    public BattleStatusEffectModel Negate()
    {
        return new BattleStatusEffectModel(
            -MaxHealth,
            -Attack,
            -Defense,
            -MovementSpeed,
            1.0f / AttackSpeedMultiplier,
            1.0f / ExperienceMultiplier
        );
    }
}
```

### 効果種別・値オブジェクト

#### EffectType
```csharp
public enum EffectType
{
    Buff = 1,        // 有利効果
    Debuff = 2,      // 不利効果
    Neutral = 3,     // 中性効果
    Environmental = 4 // 環境効果
}
```

#### EffectName
```csharp
// 【未実装】効果名の値オブジェクト
public record EffectName
{
    public string Value { get; }
    
    public EffectName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Effect name cannot be empty");
        
        if (value.Length > 50)
            throw new ArgumentException("Effect name cannot exceed 50 characters");
        
        Value = value;
    }
}
```

### 効果管理

#### EffectCollection
```csharp
// 【未実装】効果のコレクション管理システム
public record EffectCollection
{
    private readonly ImmutableArray<BattleStatusEffect> effects;
    
    public ImmutableArray<BattleStatusEffect> Effects => effects;
    public int Count => effects.Length;
    
    public EffectCollection() : this(ImmutableArray<BattleStatusEffect>.Empty) { }
    
    public EffectCollection(ImmutableArray<BattleStatusEffect> effects)
    {
        this.effects = effects;
    }
    
    public EffectCollection AddEffect(BattleStatusEffect effect)
    {
        // スタック不可効果の重複チェック
        if (!effect.IsStackable)
        {
            var existingIndex = effects.ToList().FindIndex(e => 
                e.Name.Value == effect.Name.Value);
            
            if (existingIndex >= 0)
            {
                // 既存効果を上書き
                return this with { effects = effects.SetItem(existingIndex, effect) };
            }
        }
        
        // 新規効果追加
        return this with { effects = effects.Add(effect) };
    }
    
    public EffectCollection RemoveEffect(EffectID effectId)
    {
        return this with { effects = effects.RemoveAll(e => e.Id == effectId) };
    }
    
    public EffectCollection RemoveEffectByName(EffectName effectName)
    {
        return this with { effects = effects.RemoveAll(e => e.Name.Value == effectName.Value) };
    }
    
    public EffectCollection ProcessTurn()
    {
        // 持続時間を減らし、期限切れ効果を削除
        var updatedEffects = effects
            .Select(e => e.DecreaseDuration())
            .Where(e => !e.IsExpired)
            .ToImmutableArray();
        
        return this with { effects = updatedEffects };
    }
    
    public BattleStatusEffectModel GetCombinedEffect()
    {
        var activeEffects = effects
            .Where(e => !e.IsExpired)
            .Select(e => e.StatusModel);
        
        return BattleStatusEffectModel.Combine(activeEffects);
    }
    
    public IEnumerable<BattleStatusEffect> GetEffectsByType(EffectType type)
    {
        return effects.Where(e => e.Type == type && !e.IsExpired);
    }
    
    public bool HasEffect(EffectName effectName)
    {
        return effects.Any(e => e.Name.Value == effectName.Value && !e.IsExpired);
    }
}
```

### 効果ファクトリー

#### EffectFactory
```csharp
// 【未実装】効果作成ファクトリー
public static class EffectFactory
{
    // 基本バフ効果
    public static BattleStatusEffect CreateAttackBuff(int attackBonus, int duration)
    {
        return new BattleStatusEffect(
            new EffectID(),
            new EffectName("Attack Boost"),
            new BattleStatusEffectModel(attack: attackBonus),
            duration,
            EffectType.Buff
        );
    }
    
    public static BattleStatusEffect CreateDefenseBuff(int defenseBonus, int duration)
    {
        return new BattleStatusEffect(
            new EffectID(),
            new EffectName("Defense Boost"),
            new BattleStatusEffectModel(defense: defenseBonus),
            duration,
            EffectType.Buff
        );
    }
    
    public static BattleStatusEffect CreateHealthBuff(int healthBonus, int duration)
    {
        return new BattleStatusEffect(
            new EffectID(),
            new EffectName("Health Boost"),
            new BattleStatusEffectModel(maxHealth: healthBonus),
            duration,
            EffectType.Buff
        );
    }
    
    // 基本デバフ効果
    public static BattleStatusEffect CreateAttackDebuff(int attackPenalty, int duration)
    {
        return new BattleStatusEffect(
            new EffectID(),
            new EffectName("Attack Reduction"),
            new BattleStatusEffectModel(attack: -attackPenalty),
            duration,
            EffectType.Debuff
        );
    }
    
    public static BattleStatusEffect CreateSpeedDebuff(float speedMultiplier, int duration)
    {
        return new BattleStatusEffect(
            new EffectID(),
            new EffectName("Speed Reduction"),
            new BattleStatusEffectModel(attackSpeedMultiplier: speedMultiplier),
            duration,
            EffectType.Debuff
        );
    }
    
    // 特殊効果
    public static BattleStatusEffect CreateExperienceBoost(float expMultiplier, int duration)
    {
        return new BattleStatusEffect(
            new EffectID(),
            new EffectName("Experience Boost"),
            new BattleStatusEffectModel(experienceMultiplier: expMultiplier),
            duration,
            EffectType.Buff
        );
    }
    
    // 永続効果
    public static BattleStatusEffect CreatePermanentEffect(
        EffectName name,
        BattleStatusEffectModel model,
        EffectType type)
    {
        return new BattleStatusEffect(
            new EffectID(),
            name,
            model,
            -1, // 永続
            type
        );
    }
}
```

## ビジネスルール

### 効果重複制限

#### スタッキングルール
```csharp
// 【未実装】効果重複管理システム
public static class EffectStackingRules
{
    public static bool CanStack(BattleStatusEffect existingEffect, BattleStatusEffect newEffect)
    {
        // 同名効果のスタック判定
        if (existingEffect.Name.Value == newEffect.Name.Value)
            return newEffect.IsStackable;
        
        // 異なる効果は常にスタック可能
        return true;
    }
    
    public static BattleStatusEffect ResolveConflict(
        BattleStatusEffect existingEffect, 
        BattleStatusEffect newEffect)
    {
        if (!CanStack(existingEffect, newEffect))
        {
            // 強い効果を優先（効果量・持続時間で判定）
            var existingPower = CalculateEffectPower(existingEffect);
            var newPower = CalculateEffectPower(newEffect);
            
            return newPower >= existingPower ? newEffect : existingEffect;
        }
        
        return newEffect;
    }
    
    private static int CalculateEffectPower(BattleStatusEffect effect)
    {
        var model = effect.StatusModel;
        var basePower = Math.Abs(model.MaxHealth) + Math.Abs(model.Attack) + Math.Abs(model.Defense);
        return basePower * effect.Duration;
    }
}
```

### 効果制限

#### 最大効果数制限
```csharp
// 【未実装】効果数制限システム
public static class EffectLimits
{
    public const int MaxEffectsPerCharacter = 20;
    public const int MaxSameNameEffects = 5;
    
    public static bool CanAddEffect(EffectCollection collection, BattleStatusEffect newEffect)
    {
        // 総効果数チェック
        if (collection.Count >= MaxEffectsPerCharacter)
            return false;
        
        // 同名効果数チェック
        var sameNameCount = collection.Effects.Count(e => 
            e.Name.Value == newEffect.Name.Value);
        
        if (sameNameCount >= MaxSameNameEffects)
            return false;
        
        return true;
    }
    
    public static EffectCollection EnforceLimit(EffectCollection collection)
    {
        if (collection.Count <= MaxEffectsPerCharacter)
            return collection;
        
        // 優先度による効果削除（期限切れ > デバフ > 弱いバフ）
        var prioritizedEffects = collection.Effects
            .OrderBy(e => e.IsExpired ? 0 : 1)
            .ThenBy(e => e.Type == EffectType.Debuff ? 0 : 1)
            .ThenBy(e => EffectStackingRules.CalculateEffectPower(e))
            .Take(MaxEffectsPerCharacter)
            .ToImmutableArray();
        
        return new EffectCollection(prioritizedEffects);
    }
}
```

## ゲームロジック

### 効果適用システム

```csharp
// 【未実装】効果適用システム
public static class EffectApplicationService
{
    public static EffectCollection ApplyEffect(
        EffectCollection collection,
        BattleStatusEffect effect)
    {
        // 制限チェック
        if (!EffectLimits.CanAddEffect(collection, effect))
        {
            // 制限適用後に再試行
            collection = EffectLimits.EnforceLimit(collection);
            if (!EffectLimits.CanAddEffect(collection, effect))
                throw new InvalidOperationException("Cannot add more effects");
        }
        
        // 競合解決
        var existingEffect = collection.Effects.FirstOrDefault(e => 
            e.Name.Value == effect.Name.Value);
        
        if (existingEffect != null && !effect.IsStackable)
        {
            var resolvedEffect = EffectStackingRules.ResolveConflict(existingEffect, effect);
            return collection.RemoveEffect(existingEffect.Id).AddEffect(resolvedEffect);
        }
        
        return collection.AddEffect(effect);
    }
    
    public static EffectCollection RemoveEffectsOfType(
        EffectCollection collection,
        EffectType effectType)
    {
        var effectsToRemove = collection.GetEffectsByType(effectType).ToList();
        
        var result = collection;
        foreach (var effect in effectsToRemove)
        {
            result = result.RemoveEffect(effect.Id);
        }
        
        return result;
    }
    
    public static EffectCollection CleanseDebuffs(EffectCollection collection)
    {
        return RemoveEffectsOfType(collection, EffectType.Debuff);
    }
    
    public static EffectCollection ProcessTimeBasedEffects(EffectCollection collection)
    {
        return collection.ProcessTurn();
    }
}
```

### 戦闘効果統合

```csharp
// 【未実装】戦闘パラメータへの効果統合システム
public static class CombatEffectIntegration
{
    public static BattleParameter ApplyEffectsToParameters(
        BattleParameter baseParameters,
        EffectCollection effects)
    {
        var combinedEffect = effects.GetCombinedEffect();
        
        return new BattleParameter(
            Math.Max(1, baseParameters.MaxHealth + combinedEffect.MaxHealth),
            Math.Max(1, baseParameters.AttackValue + combinedEffect.Attack),
            Math.Max(0, baseParameters.DefenseValue + combinedEffect.Defense)
        );
    }
    
    public static float CalculateAttackSpeed(
        float baseAttackSpeed,
        EffectCollection effects)
    {
        var combinedEffect = effects.GetCombinedEffect();
        return Math.Max(0.1f, baseAttackSpeed * combinedEffect.AttackSpeedMultiplier);
    }
    
    public static float CalculateExperienceMultiplier(EffectCollection effects)
    {
        var combinedEffect = effects.GetCombinedEffect();
        return Math.Max(0.0f, combinedEffect.ExperienceMultiplier);
    }
}
```

### 効果管理

```csharp
// 【未実装】効果のライフサイクル管理サービス
public static class EffectManagementService
{
    public static EffectCollection StartCombat(EffectCollection effects)
    {
        // 戦闘開始時の効果処理
        // 例: 戦闘外効果の除去、戦闘開始バフの適用など
        return effects;
    }
    
    public static EffectCollection EndCombat(EffectCollection effects)
    {
        // 戦闘終了時の効果処理
        // 例: 戦闘専用効果の除去
        var combatOnlyEffects = effects.Effects
            .Where(e => e.Name.Value.Contains("Combat"))
            .ToList();
        
        var result = effects;
        foreach (var effect in combatOnlyEffects)
        {
            result = result.RemoveEffect(effect.Id);
        }
        
        return result;
    }
    
    public static EffectCollection ApplyAreaEffect(
        EffectCollection effects,
        FieldID currentField)
    {
        // エリア固有の環境効果を適用
        // 例: 毒沼地での毒効果、聖域での回復効果など
        
        // 実装例（将来拡張）
        return effects;
    }
}
```

## 他ドメインとの連携

### Character ドメインとの連携
- **戦闘パラメータ**: 効果による攻撃力・防御力・体力の補正
- **経験値獲得**: 経験値倍率効果の適用
- 詳細: [BattleSystem.md](../systems/BattleSystem.md)

### Skill ドメインとの連携
- **スキル効果**: 戦闘スキル・生産スキルによる効果付与
- **熟練度補正**: スキル熟練度による効果強化
- 詳細: [Skill.md](./Skill.md)

### Item ドメインとの連携
- **装備効果**: 装備品による永続的ステータス効果
- **消耗品効果**: ポーション等による一時的効果（将来拡張）
- 詳細: [Item.md](./Item.md)

### Field ドメインとの連携
- **環境効果**: エリア固有の効果（天候、地形等）
- **位置効果**: 特定位置での効果発動（将来拡張）
- 詳細: [Field.md](./Field.md)

## 拡張ポイント

### 条件付き効果システム
```csharp
// 【未実装】特定条件下でのみ発動する効果システム
public record ConditionalEffect : BattleStatusEffect
{
    public IEffectCondition Condition { get; }
    public bool IsActive(GameState gameState) => Condition.IsMet(gameState);
}

public interface IEffectCondition
{
    bool IsMet(GameState gameState);
}
```

### 効果連鎖システム
```csharp
// 【未実装】効果の終了時に別の効果を発動するシステム
public record ChainedEffect : BattleStatusEffect
{
    public BattleStatusEffect? NextEffect { get; }
    
    public BattleStatusEffect OnExpire()
    {
        return NextEffect ?? throw new InvalidOperationException("No chained effect");
    }
}
```

### 効果レベルシステム
```csharp
// 【未実装】効果の強度レベルシステム
public record LeveledEffect : BattleStatusEffect
{
    public int Level { get; }
    public int MaxLevel { get; }
    
    public LeveledEffect UpgradeLevel()
    {
        if (Level >= MaxLevel) return this;
        
        var enhancedModel = EnhanceEffectModel(StatusModel, Level + 1);
        return this with 
        { 
            Level = Level + 1,
            StatusModel = enhancedModel
        };
    }
}
```

### 効果相互作用システム
```csharp
// 【未実装】複数効果間の相互作用システム
public static class EffectSynergy
{
    public static BattleStatusEffect? CheckSynergy(
        IEnumerable<BattleStatusEffect> effects)
    {
        // 特定の効果組み合わせによる追加効果
        // 例: 攻撃バフ + 速度バフ = クリティカル率上昇
        return null;
    }
}
```

PassiveEffect ドメインは、ゲームの戦術性と深さを提供する重要なシステムです。\
他のドメインと連携することで、豊富なゲームプレイ体験を実現します。