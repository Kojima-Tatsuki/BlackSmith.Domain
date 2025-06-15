# 戦闘システム (Battle System)

## 概要

戦闘システムは、Character、Item、PassiveEffect の3つのドメインを統合し、プレイヤーの戦闘能力を総合的に計算・管理するシステムです。\
基本ステータス、装備効果、ステータス効果を組み合わせた戦闘パラメータの算出とダメージ処理を担います。

## システム構成

### 関連ドメイン

- **[Character](../domains/Character.md)**: 基本ステータス、レベル、経験値
- **[Item](../domains/Item.md)**: 装備品、強化パラメータ
- **[PassiveEffect](../domains/PassiveEffect.md)**: バフ・デバフ、ステータス効果

### 統合モジュール

#### CharacterBattleModule
複数ドメインの戦闘関連機能を統合する中核モジュール。

```csharp
public class CharacterBattleModule
{
    private readonly BattleEquipmentModule equipmentModule;
    private readonly BattleStatusEffectModule statusEffectModule;
    
    public CharacterBattleModule(
        BattleEquipmentModule equipmentModule,
        BattleStatusEffectModule statusEffectModule)
    {
        this.equipmentModule = equipmentModule;
        this.statusEffectModule = statusEffectModule;
    }
    
    public BattleParameter GetBattleParameter(PlayerCommonEntity player)
    {
        // 基本パラメータ（Character ドメイン）
        var baseParam = CalculateBaseParameter(player);
        
        // 装備補正（Item ドメイン）
        var equipmentParam = equipmentModule.GetEquipmentParameter();
        
        // ステータス効果（PassiveEffect ドメイン）
        var effectParam = statusEffectModule.GetStatusEffectParameter();
        
        return CombineBattleParameters(baseParam, equipmentParam, effectParam);
    }
    
    private BattleParameter CalculateBaseParameter(PlayerCommonEntity player)
    {
        var maxHealth = player.Level.Value * 10;
        var attackValue = (player.Strength.Value + player.Agility.Value) * 2;
        var defenseValue = (player.Strength.Value + player.Agility.Value) * 2;
        
        return new BattleParameter(maxHealth, attackValue, defenseValue);
    }
    
    private BattleParameter CombineBattleParameters(
        BattleParameter baseParam,
        BattleParameter equipmentParam,
        BattleParameter effectParam)
    {
        return new BattleParameter(
            Math.Max(1, baseParam.MaxHealth + equipmentParam.MaxHealth + effectParam.MaxHealth),
            Math.Max(1, baseParam.AttackValue + equipmentParam.AttackValue + effectParam.AttackValue),
            Math.Max(0, baseParam.DefenseValue + equipmentParam.DefenseValue + effectParam.DefenseValue)
        );
    }
}
```

## 戦闘パラメータ計算

### 総合戦闘力算出

```csharp
public static class BattleParameterCalculator
{
    public static BattleParameter CalculateFinalParameters(
        PlayerCommonEntity player,
        EquipmentInventory equipment,
        EffectCollection effects)
    {
        // 1. 基本パラメータ（レベル・ステータス依存）
        var baseParams = CalculateBaseParameters(player);
        
        // 2. 装備補正
        var equipmentBonus = CalculateEquipmentBonus(equipment);
        
        // 3. ステータス効果補正
        var effectBonus = CalculateEffectBonus(effects);
        
        // 4. 統合計算
        return new BattleParameter(
            maxHealth: Math.Max(1, baseParams.MaxHealth + equipmentBonus.MaxHealth + effectBonus.MaxHealth),
            attackValue: Math.Max(1, baseParams.AttackValue + equipmentBonus.AttackValue + effectBonus.AttackValue),
            defenseValue: Math.Max(0, baseParams.DefenseValue + equipmentBonus.DefenseValue + effectBonus.DefenseValue)
        );
    }
    
    private static BattleParameter CalculateBaseParameters(PlayerCommonEntity player)
    {
        // Character ドメインの計算ルール
        return new BattleParameter(
            maxHealth: player.Level.Value * 10,
            attackValue: (player.Strength.Value + player.Agility.Value) * 2,
            defenseValue: (player.Strength.Value + player.Agility.Value) * 2
        );
    }
    
    private static BattleParameter CalculateEquipmentBonus(EquipmentInventory equipment)
    {
        // Item ドメインの装備効果計算
        var weaponAttack = equipment.WeaponSlot?.Enhancement.GetAttackValue() ?? 0;
        var weaponDefense = equipment.WeaponSlot?.Enhancement.GetDefenseValue() ?? 0;
        
        var armorAttack = equipment.ArmorSlot?.Enhancement.GetAttackValue() ?? 0;
        var armorDefense = equipment.ArmorSlot?.Enhancement.GetDefenseValue() ?? 0;
        
        var accessaryAttack = equipment.AccessarySlot?.Enhancement.GetAttackValue() ?? 0;
        var accessaryDefense = equipment.AccessarySlot?.Enhancement.GetDefenseValue() ?? 0;
        
        return new BattleParameter(
            maxHealth: 0, // 装備による体力ボーナスはなし（将来拡張可能）
            attackValue: weaponAttack + armorAttack + accessaryAttack,
            defenseValue: weaponDefense + armorDefense + accessaryDefense
        );
    }
    
    private static BattleParameter CalculateEffectBonus(EffectCollection effects)
    {
        // PassiveEffect ドメインの効果統合
        var combinedEffect = effects.GetCombinedEffect();
        
        return new BattleParameter(
            maxHealth: combinedEffect.MaxHealth,
            attackValue: combinedEffect.Attack,
            defenseValue: combinedEffect.Defense
        );
    }
}
```

### 特殊補正システム

```csharp
public static class BattleModifierSystem
{
    // 攻撃速度計算（PassiveEffect による補正）
    public static float CalculateAttackSpeed(
        float baseAttackSpeed,
        EffectCollection effects)
    {
        var combinedEffect = effects.GetCombinedEffect();
        return Math.Max(0.1f, baseAttackSpeed * combinedEffect.AttackSpeedMultiplier);
    }
    
    // クリティカル率計算（Skill による補正）
    public static float CalculateCriticalRate(
        PlayerCommonEntity player,
        IEnumerable<BattleSkill> battleSkills)
    {
        var baseCriticalRate = 0.05f; // 基本5%
        
        var skillBonus = battleSkills
            .Where(skill => player.Skills.Any(ps => ps.Skill.Name.Value == skill.Name.Value))
            .Sum(skill => {
                var playerSkill = player.Skills.First(ps => ps.Skill.Name.Value == skill.Name.Value);
                var proficiencyMultiplier = playerSkill.Proficiency.Value / 1000.0f;
                return skill.Effect.CriticalRateBonus * proficiencyMultiplier;
            });
        
        return Math.Min(1.0f, baseCriticalRate + skillBonus / 100.0f);
    }
    
    // 命中率計算
    public static float CalculateAccuracy(
        PlayerCommonEntity player,
        EquipmentInventory equipment)
    {
        var baseAccuracy = 0.9f; // 基本90%
        var agilityBonus = player.Agility.Value * 0.001f; // AGI1につき0.1%向上
        
        // 装備による命中補正（将来拡張）
        var equipmentBonus = 0.0f;
        
        return Math.Min(1.0f, baseAccuracy + agilityBonus + equipmentBonus);
    }
}
```

## ダメージ計算システム

### 基本ダメージ計算

```csharp
public static class DamageCalculator
{
    public static DamageResult CalculateDamage(
        BattleParameter attackerParams,
        BattleParameter defenderParams,
        bool isCritical = false,
        float accuracyRate = 1.0f)
    {
        // 命中判定
        var random = new Random();
        if (random.NextDouble() > accuracyRate)
        {
            return DamageResult.Miss();
        }
        
        // 基本ダメージ計算
        var baseDamage = Math.Max(1, attackerParams.AttackValue - defenderParams.DefenseValue);
        
        // クリティカル補正
        var finalDamage = isCritical ? (int)(baseDamage * 1.5f) : baseDamage;
        
        return DamageResult.Hit(finalDamage, isCritical);
    }
    
    public static PlayerBattleEntity ApplyDamage(
        PlayerBattleEntity battleEntity,
        int damage)
    {
        var newHealth = battleEntity.CurrentHealth.TakeDamage(damage);
        return battleEntity with { CurrentHealth = newHealth };
    }
    
    public static PlayerBattleEntity Heal(
        PlayerBattleEntity battleEntity,
        int healAmount)
    {
        var maxHealth = battleEntity.BattleModule.GetBattleParameter(/* player */).MaxHealth;
        var newHealth = battleEntity.CurrentHealth.Heal(healAmount, maxHealth);
        return battleEntity with { CurrentHealth = newHealth };
    }
}

public record DamageResult
{
    public bool IsHit { get; }
    public int Damage { get; }
    public bool IsCritical { get; }
    
    private DamageResult(bool isHit, int damage, bool isCritical)
    {
        IsHit = isHit;
        Damage = damage;
        IsCritical = isCritical;
    }
    
    public static DamageResult Hit(int damage, bool isCritical = false) =>
        new DamageResult(true, damage, isCritical);
    
    public static DamageResult Miss() =>
        new DamageResult(false, 0, false);
}
```

### 状態異常・効果適用

```csharp
public static class BattleEffectProcessor
{
    public static EffectCollection ProcessBattleStart(EffectCollection effects)
    {
        // 戦闘開始時の効果処理
        var battleStartEffect = EffectFactory.CreateAttackBuff(5, 5);
        return effects.AddEffect(battleStartEffect);
    }
    
    public static EffectCollection ProcessBattleEnd(EffectCollection effects)
    {
        // 戦闘終了時の効果クリア
        return EffectApplicationService.RemoveEffectsOfType(effects, EffectType.Neutral);
    }
    
    public static EffectCollection ProcessTurnStart(EffectCollection effects)
    {
        // ターン開始時の効果処理（持続時間減少）
        return effects.ProcessTurn();
    }
    
    public static (PlayerBattleEntity updatedEntity, EffectCollection updatedEffects) 
        ProcessDamageOverTime(
            PlayerBattleEntity battleEntity,
            EffectCollection effects)
    {
        // 継続ダメージ効果の処理
        var damageEffects = effects.GetEffectsByType(EffectType.Debuff)
            .Where(e => e.Name.Value.Contains("Poison") || e.Name.Value.Contains("Burn"));
        
        var totalDamage = damageEffects.Sum(e => Math.Abs(e.StatusModel.Attack));
        
        if (totalDamage > 0)
        {
            var updatedEntity = DamageCalculator.ApplyDamage(battleEntity, totalDamage);
            return (updatedEntity, effects);
        }
        
        return (battleEntity, effects);
    }
}
```

## 戦闘フロー管理

### 戦闘シーケンス

```csharp
public class BattleSequence
{
    public PlayerBattleEntity Player { get; private set; }
    public EffectCollection PlayerEffects { get; private set; }
    public bool IsBattleActive { get; private set; }
    
    public BattleSequence(PlayerBattleEntity player, EffectCollection effects)
    {
        Player = player;
        PlayerEffects = effects;
        IsBattleActive = false;
    }
    
    public void StartBattle()
    {
        if (IsBattleActive)
            throw new InvalidOperationException("Battle is already active");
        
        IsBattleActive = true;
        PlayerEffects = BattleEffectProcessor.ProcessBattleStart(PlayerEffects);
    }
    
    public void EndBattle()
    {
        if (!IsBattleActive)
            throw new InvalidOperationException("No active battle to end");
        
        IsBattleActive = false;
        PlayerEffects = BattleEffectProcessor.ProcessBattleEnd(PlayerEffects);
    }
    
    public void ProcessTurn()
    {
        if (!IsBattleActive)
            throw new InvalidOperationException("No active battle");
        
        // ターン開始処理
        PlayerEffects = BattleEffectProcessor.ProcessTurnStart(PlayerEffects);
        
        // 継続ダメージ処理
        (Player, PlayerEffects) = BattleEffectProcessor.ProcessDamageOverTime(Player, PlayerEffects);
        
        // 戦闘不能チェック
        if (Player.CurrentHealth.Value <= 0)
        {
            EndBattle();
        }
    }
    
    public DamageResult AttackEnemy(BattleParameter enemyParams)
    {
        if (!IsBattleActive)
            throw new InvalidOperationException("No active battle");
        
        // プレイヤーの戦闘パラメータ計算
        var playerParams = Player.BattleModule.GetBattleParameter(/* PlayerCommonEntity */);
        
        // クリティカル判定
        var criticalRate = BattleModifierSystem.CalculateCriticalRate(/* player, skills */);
        var isCritical = new Random().NextDouble() < criticalRate;
        
        // ダメージ計算
        return DamageCalculator.CalculateDamage(playerParams, enemyParams, isCritical);
    }
    
    public void ReceiveDamage(int damage)
    {
        if (!IsBattleActive)
            throw new InvalidOperationException("No active battle");
        
        Player = DamageCalculator.ApplyDamage(Player, damage);
        
        if (Player.CurrentHealth.Value <= 0)
        {
            EndBattle();
        }
    }
}
```

## 戦闘AI・戦略

### 戦闘戦略システム（将来拡張）

```csharp
public interface IBattleStrategy
{
    BattleAction DecideBattleAction(BattleContext context);
}

public record BattleAction
{
    public BattleActionType Type { get; }
    public IItem? UsedItem { get; }
    public Skill? UsedSkill { get; }
    
    public static BattleAction Attack() => new BattleAction(BattleActionType.Attack);
    public static BattleAction UseItem(IItem item) => new BattleAction(BattleActionType.UseItem, item);
    public static BattleAction UseSkill(Skill skill) => new BattleAction(BattleActionType.UseSkill, UsedSkill: skill);
}

public enum BattleActionType
{
    Attack,
    UseItem,
    UseSkill,
    Defend,
    Flee
}

public record BattleContext
{
    public PlayerBattleEntity Player { get; }
    public EffectCollection PlayerEffects { get; }
    public BattleParameter EnemyParams { get; }
    public int TurnCount { get; }
}
```

## パフォーマンス最適化

### 計算キャッシュシステム

```csharp
public class BattleParameterCache
{
    private readonly Dictionary<string, (BattleParameter parameters, DateTime calculated)> cache;
    private readonly TimeSpan cacheExpiry = TimeSpan.FromSeconds(5);
    
    public BattleParameterCache()
    {
        cache = new Dictionary<string, (BattleParameter, DateTime)>();
    }
    
    public BattleParameter GetCachedParameters(
        PlayerID playerId,
        Func<BattleParameter> calculator)
    {
        var key = playerId.Value;
        var now = DateTime.UtcNow;
        
        if (cache.TryGetValue(key, out var cached) && 
            now - cached.calculated < cacheExpiry)
        {
            return cached.parameters;
        }
        
        var parameters = calculator();
        cache[key] = (parameters, now);
        
        return parameters;
    }
    
    public void InvalidateCache(PlayerID playerId)
    {
        cache.Remove(playerId.Value);
    }
}
```

## 拡張ポイント

### エレメント属性システム
```csharp
public enum ElementType
{
    Fire, Water, Earth, Air, Light, Dark
}

public record ElementalDamage
{
    public ElementType Element { get; }
    public int Damage { get; }
    public float Multiplier { get; }
}
```

### コンボシステム
```csharp
public record ComboAttack
{
    public ImmutableArray<Skill> RequiredSkills { get; }
    public float DamageMultiplier { get; }
    public BattleStatusEffect? ComboEffect { get; }
}
```

戦闘システムは、BlackSmith.Domain の複数ドメインを統合する中核システムとして、豊富で戦略的なゲームプレイを提供します。