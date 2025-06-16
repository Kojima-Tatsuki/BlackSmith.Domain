# Skill ドメイン

## 概要

Skill ドメインは、プレイヤーのスキルシステムを管理します。\
戦闘スキルと生産スキルの2つの主要カテゴリーがあり、それぞれ習得条件、熟練度、経験値システムを持ちます。

## ドメインモデル

### 基底クラス・インターフェース

#### Skill（抽象基底クラス）
```csharp
// 【部分実装】基本的なSkillクラスは実装済みだが、record型ではなくclass型
public abstract record Skill
{
    public SkillName Name { get; }
    public abstract SkillType Type { get; }
    public SkillAcquisitionConditions AcquisitionConditions { get; }
    
    protected Skill(SkillName name, SkillAcquisitionConditions acquisitionConditions)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        AcquisitionConditions = acquisitionConditions ?? throw new ArgumentNullException(nameof(acquisitionConditions));
    }
}
```

### スキル種別

#### BattleSkill（戦闘スキル）
```csharp
// 【部分実装】基本的なBattleSkillクラスは実装済みだが、このrecord型モデルは未実装
public record BattleSkill : Skill
{
    public override SkillType Type => SkillType.Battle;
    public BattleSkillEffect Effect { get; }
    
    public BattleSkill(
        SkillName name, 
        SkillAcquisitionConditions acquisitionConditions,
        BattleSkillEffect effect) 
        : base(name, acquisitionConditions)
    {
        Effect = effect;
    }
}

// 【未実装】戦闘スキル効果システム
public record BattleSkillEffect
{
    public int AttackBonus { get; }
    public int DefenseBonus { get; }
    public int AccuracyBonus { get; }
    public int CriticalRateBonus { get; }
    
    public BattleSkillEffect(int attackBonus = 0, int defenseBonus = 0, int accuracyBonus = 0, int criticalRateBonus = 0)
    {
        AttackBonus = Math.Max(0, attackBonus);
        DefenseBonus = Math.Max(0, defenseBonus);
        AccuracyBonus = Math.Max(0, accuracyBonus);
        CriticalRateBonus = Math.Max(0, criticalRateBonus);
    }
}
```

#### ProductionSkill（生産スキル）
```csharp
// 【部分実装】基本的なProductionSkillクラスは実装済みだが、このrecord型モデルは未実装
public record ProductionSkill : Skill
{
    public override SkillType Type => SkillType.Production;
    public ProductionType ProductionType { get; }
    
    public ProductionSkill(
        SkillName name, 
        SkillAcquisitionConditions acquisitionConditions,
        ProductionType productionType) 
        : base(name, acquisitionConditions)
    {
        ProductionType = productionType;
    }
}

public enum ProductionType
{
    Creation = 1,  // 作成（新規アイテム生産）
    Refining = 2,  // 精錬（素材加工）
    Repair = 3     // 修理（装備修復）
}

public enum SkillType
{
    Battle = 1,
    Production = 2
}
```

### 値オブジェクト

#### SkillName
```csharp
// 【実装済み】スキル名の値オブジェクト
public record SkillName
{
    public string Value { get; }
    
    public SkillName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Skill name cannot be empty");
        
        if (value.Length > 50)
            throw new ArgumentException("Skill name cannot exceed 50 characters");
        
        Value = value;
    }
}
```

#### SkillProficiency（熟練度）
```csharp
// 【実装済み】スキル熟練度の管理クラス
public record SkillProficiency
{
    public int Value { get; }
    
    public SkillProficiency(int value)
    {
        if (value < 1 || value > 1000)
            throw new ArgumentOutOfRangeException("Proficiency must be between 1 and 1000");
        
        Value = value;
    }
    
    public SkillRank GetRank() => Value switch
    {
        >= 1 and <= 100 => SkillRank.Novice,
        >= 101 and <= 300 => SkillRank.Intermediate,
        >= 301 and <= 600 => SkillRank.Advanced,
        >= 601 and <= 900 => SkillRank.Expert,
        >= 901 and <= 1000 => SkillRank.Master,
        _ => throw new InvalidOperationException("Invalid proficiency value")
    };
}

public enum SkillRank
{
    Novice = 1,       // 初心者 (1-100)
    Intermediate = 2, // 中級者 (101-300)
    Advanced = 3,     // 上級者 (301-600)
    Expert = 4,       // 熟練者 (601-900)
    Master = 5        // 達人 (901-1000)
}
```

#### SkillExperience（スキル経験値）
```csharp
// 【実装済み】スキル経験値の管理クラス
public record SkillExperience
{
    public int Value { get; }
    
    public SkillExperience(int value)
    {
        if (value < 0)
            throw new ArgumentException("Experience cannot be negative");
        
        Value = value;
    }
    
    public SkillProficiency CalculateProficiency()
    {
        // 経験値100につき熟練度1上昇
        var proficiency = Math.Min(1000, Math.Max(1, Value / 100));
        return new SkillProficiency(proficiency);
    }
    
    public SkillExperience AddExperience(int amount)
    {
        if (amount < 0)
            throw new ArgumentException("Experience amount cannot be negative");
        
        return new SkillExperience(Value + amount);
    }
}
```

#### SkillAndProficiency（スキル・熟練度セット）
```csharp
// 【実装済み】スキルと熟練度の組み合わせ
public record SkillAndProficiency
{
    public Skill Skill { get; }
    public SkillProficiency Proficiency { get; }
    
    public SkillAndProficiency(Skill skill, SkillProficiency proficiency)
    {
        Skill = skill ?? throw new ArgumentNullException(nameof(skill));
        Proficiency = proficiency ?? throw new ArgumentNullException(nameof(proficiency));
    }
    
    public SkillAndProficiency IncreaseProficiency(int amount)
    {
        var newValue = Math.Min(1000, Proficiency.Value + amount);
        return this with { Proficiency = new SkillProficiency(newValue) };
    }
}
```

### 習得条件

#### SkillAcquisitionConditions
```csharp
// 【部分実装】基本的な取得条件は実装済みだが、詳細な条件チェックは不完全
public record SkillAcquisitionConditions
{
    public PlayerLevel Level { get; }
    public Strength Strength { get; }
    public Agility Agility { get; }
    public IReadOnlyCollection<SkillAndProficiency> RequiredSkills { get; }
    
    public SkillAcquisitionConditions(
        PlayerLevel level,
        Strength strength,
        Agility agility,
        IReadOnlyCollection<SkillAndProficiency>? requiredSkills = null)
    {
        Level = level ?? throw new ArgumentNullException(nameof(level));
        Strength = strength ?? throw new ArgumentNullException(nameof(strength));
        Agility = agility ?? throw new ArgumentNullException(nameof(agility));
        RequiredSkills = requiredSkills ?? Array.Empty<SkillAndProficiency>();
    }
    
    public static SkillAcquisitionConditions CreateBasic(int level, int strength = 1, int agility = 1)
    {
        return new SkillAcquisitionConditions(
            new PlayerLevel(level),
            new Strength(strength),
            new Agility(agility)
        );
    }
}
```

## ビジネスルール

### スキル習得条件

#### 条件チェック
```csharp
// 【未実装】詳細なスキル取得条件チェックシステム
public static class SkillAcquisitionChecker
{
    public static bool CanAcquireSkill(Skill skill, PlayerCommonEntity player)
    {
        var conditions = skill.AcquisitionConditions;
        
        // レベル条件
        if (player.Level.Value < conditions.Level.Value)
            return false;
        
        // ステータス条件
        if (player.Strength.Value < conditions.Strength.Value ||
            player.Agility.Value < conditions.Agility.Value)
            return false;
        
        // 前提スキル条件
        foreach (var required in conditions.RequiredSkills)
        {
            var playerSkill = player.Skills.FirstOrDefault(s => 
                s.Skill.Name.Value == required.Skill.Name.Value);
            
            if (playerSkill == null || playerSkill.Proficiency.Value < required.Proficiency.Value)
                return false;
        }
        
        return true;
    }
    
    public static string GetFailureReason(Skill skill, PlayerCommonEntity player)
    {
        var conditions = skill.AcquisitionConditions;
        
        if (player.Level.Value < conditions.Level.Value)
            return $"Required level: {conditions.Level.Value} (Current: {player.Level.Value})";
        
        if (player.Strength.Value < conditions.Strength.Value)
            return $"Required strength: {conditions.Strength.Value} (Current: {player.Strength.Value})";
        
        if (player.Agility.Value < conditions.Agility.Value)
            return $"Required agility: {conditions.Agility.Value} (Current: {player.Agility.Value})";
        
        foreach (var required in conditions.RequiredSkills)
        {
            var playerSkill = player.Skills.FirstOrDefault(s => 
                s.Skill.Name.Value == required.Skill.Name.Value);
            
            if (playerSkill == null)
                return $"Required skill: {required.Skill.Name.Value}";
            
            if (playerSkill.Proficiency.Value < required.Proficiency.Value)
                return $"Required {required.Skill.Name.Value} proficiency: {required.Proficiency.Value} (Current: {playerSkill.Proficiency.Value})";
        }
        
        return "All conditions met";
    }
}
```

### 熟練度システム

#### 経験値獲得・熟練度上昇
```csharp
// 【部分実装】基本的な経験値計算は実装済みだが、この統合システムは未実装
public static class SkillExperienceCalculator
{
    public static int CalculateExperienceGain(SkillType skillType, int actionDifficulty, int playerLevel)
    {
        var baseExperience = skillType switch
        {
            SkillType.Battle => 50,      // 戦闘スキル基本経験値
            SkillType.Production => 30,  // 生産スキル基本経験値
            _ => 25
        };
        
        // 難易度補正
        var difficultyMultiplier = actionDifficulty switch
        {
            <= 1 => 0.5f,  // 簡単
            2 => 0.75f,    // 普通
            3 => 1.0f,     // 標準
            4 => 1.25f,    // 困難
            >= 5 => 1.5f   // 極困難
        };
        
        // レベル補正（高レベルほど経験値効率低下）
        var levelPenalty = Math.Max(0.1f, 1.0f - (playerLevel - 1) * 0.01f);
        
        return (int)(baseExperience * difficultyMultiplier * levelPenalty);
    }
    
    public static SkillExperience ProcessExperienceGain(
        SkillExperience currentExperience, 
        int gainedExperience)
    {
        return currentExperience.AddExperience(gainedExperience);
    }
}
```

### スキル効果

#### 戦闘スキル効果計算
```csharp
// 【未実装】戦闘スキル効果計算システム
public static class BattleSkillEffectCalculator
{
    public static BattleSkillEffect CalculateEffect(BattleSkill skill, SkillProficiency proficiency)
    {
        var baseEffect = skill.Effect;
        var proficiencyMultiplier = proficiency.Value / 1000.0f; // 0.001 - 1.0
        
        return new BattleSkillEffect(
            (int)(baseEffect.AttackBonus * proficiencyMultiplier),
            (int)(baseEffect.DefenseBonus * proficiencyMultiplier),
            (int)(baseEffect.AccuracyBonus * proficiencyMultiplier),
            (int)(baseEffect.CriticalRateBonus * proficiencyMultiplier)
        );
    }
    
    public static BattleSkillEffect CombineEffects(IEnumerable<BattleSkillEffect> effects)
    {
        return effects.Aggregate(
            new BattleSkillEffect(),
            (acc, effect) => new BattleSkillEffect(
                acc.AttackBonus + effect.AttackBonus,
                acc.DefenseBonus + effect.DefenseBonus,
                acc.AccuracyBonus + effect.AccuracyBonus,
                acc.CriticalRateBonus + effect.CriticalRateBonus
            )
        );
    }
}
```

## ゲームロジック

### スキル習得プロセス

```csharp
// 【未実装】スキル習得管理サービス
public static class SkillLearningService
{
    public static PlayerCommonEntity LearnSkill(
        PlayerCommonEntity player, 
        Skill skill, 
        SkillProficiency initialProficiency)
    {
        // 既存スキルチェック
        if (player.Skills.Any(s => s.Skill.Name.Value == skill.Name.Value))
            throw new InvalidOperationException("Skill already learned");
        
        // 習得条件チェック
        if (!SkillAcquisitionChecker.CanAcquireSkill(skill, player))
        {
            var reason = SkillAcquisitionChecker.GetFailureReason(skill, player);
            throw new InvalidOperationException($"Cannot learn skill: {reason}");
        }
        
        // スキル追加
        var newSkill = new SkillAndProficiency(skill, initialProficiency);
        return player with { Skills = player.Skills.Add(newSkill) };
    }
    
    public static PlayerCommonEntity ImproveSkill(
        PlayerCommonEntity player,
        SkillName skillName,
        int experienceGained)
    {
        var skillIndex = player.Skills.ToList().FindIndex(s => 
            s.Skill.Name.Value == skillName.Value);
        
        if (skillIndex < 0)
            throw new InvalidOperationException("Skill not found");
        
        var currentSkill = player.Skills[skillIndex];
        var newProficiency = Math.Min(1000, currentSkill.Proficiency.Value + experienceGained);
        var updatedSkill = currentSkill with 
        { 
            Proficiency = new SkillProficiency(newProficiency) 
        };
        
        return player with 
        { 
            Skills = player.Skills.SetItem(skillIndex, updatedSkill) 
        };
    }
}
```

### 生産スキル活用

```csharp
// 【未実装】生産スキル活用サービス
public static class ProductionSkillService
{
    public static bool CanCraftItem(
        ICraftableItem item, 
        PlayerCommonEntity player, 
        ProductionType requiredProductionType)
    {
        // 必要な生産スキルの確認
        var requiredSkill = player.Skills.FirstOrDefault(s => 
            s.Skill is ProductionSkill ps && ps.ProductionType == requiredProductionType);
        
        if (requiredSkill == null)
            return false;
        
        // 熟練度による成功率計算
        var successRate = CalculateCraftSuccessRate(requiredSkill.Proficiency, item);
        return successRate > 0.5f; // 50%以上の成功率が必要
    }
    
    public static float CalculateCraftSuccessRate(SkillProficiency proficiency, ICraftableItem item)
    {
        // アイテムの複雑度（将来実装）
        var itemComplexity = 100; // 仮の値
        
        // 熟練度による成功率
        var baseSuccessRate = proficiency.Value / 1000.0f;
        var difficultyPenalty = Math.Max(0.1f, 1.0f - (itemComplexity / 1000.0f));
        
        return Math.Min(1.0f, baseSuccessRate * difficultyPenalty);
    }
    
    public static int CalculateCraftExperience(ICraftableItem item, bool wasSuccessful)
    {
        var baseExperience = 30;
        var successMultiplier = wasSuccessful ? 1.0f : 0.5f; // 失敗時は半分の経験値
        
        return (int)(baseExperience * successMultiplier);
    }
}
```

### スキル管理

```csharp
// 【未実装】統合スキル管理システム
public static class SkillManager
{
    public static IEnumerable<Skill> GetLearnableSkills(PlayerCommonEntity player, IEnumerable<Skill> allSkills)
    {
        return allSkills.Where(skill => 
            !player.Skills.Any(s => s.Skill.Name.Value == skill.Name.Value) &&
            SkillAcquisitionChecker.CanAcquireSkill(skill, player)
        );
    }
    
    public static IEnumerable<SkillAndProficiency> GetSkillsByType(PlayerCommonEntity player, SkillType skillType)
    {
        return player.Skills.Where(s => s.Skill.Type == skillType);
    }
    
    public static SkillAndProficiency? GetSkillByName(PlayerCommonEntity player, SkillName skillName)
    {
        return player.Skills.FirstOrDefault(s => s.Skill.Name.Value == skillName.Value);
    }
    
    public static int GetTotalSkillCount(PlayerCommonEntity player)
    {
        return player.Skills.Length;
    }
    
    public static int GetAverageProficiency(PlayerCommonEntity player, SkillType? skillType = null)
    {
        var skills = skillType.HasValue 
            ? player.Skills.Where(s => s.Skill.Type == skillType.Value)
            : player.Skills;
        
        return skills.Any() ? (int)skills.Average(s => s.Proficiency.Value) : 0;
    }
}
```

## 他ドメインとの連携

### Character ドメインとの連携
- **スキル習得**: レベル・ステータス条件チェック
- **戦闘効果**: 戦闘スキルによる能力値補正
- 詳細: [Character.md](./Character.md)

### Item ドメインとの連携
- **クラフト条件**: 生産スキル熟練度による成功率
- **装備条件**: 特定スキル習得による装備解放（将来拡張）
- 詳細: [CraftingSystem.md](../systems/CraftingSystem.md)

### Inventory ドメインとの連携
- **素材管理**: クラフト時の素材消費・生成
- **ツール要求**: 生産スキル使用時の専用ツール（将来拡張）
- 詳細: [Inventory.md](./Inventory.md)

## 拡張ポイント

### スキルツリーシステム
```csharp
// 【未実装】スキル間の依存関係をツリー構造で管理
public record SkillTree
{
    public SkillTreeNode Root { get; }
    public ImmutableArray<SkillTreeNode> AllNodes { get; }
}

public record SkillTreeNode
{
    public Skill Skill { get; }
    public ImmutableArray<SkillTreeNode> Prerequisites { get; }
    public ImmutableArray<SkillTreeNode> Unlocks { get; }
}
```

### スキルコンボシステム
```csharp
// 【未実装】複数スキルの組み合わせ効果
public record SkillCombo
{
    public string Name { get; }
    public ImmutableArray<SkillName> RequiredSkills { get; }
    public BattleSkillEffect ComboEffect { get; }
}
```

### スキル特化システム
```csharp
// 【未実装】スキルの特化方向システム
public enum SkillSpecialization
{
    Damage,      // ダメージ特化
    Defense,     // 防御特化
    Utility,     // 補助特化
    Efficiency   // 効率特化
}

public record SpecializedSkill : Skill
{
    public SkillSpecialization Specialization { get; }
    public int SpecializationLevel { get; }
}
```

### パッシブスキルシステム
```csharp
// 【未実装】常時発動するパッシブスキル
public record PassiveSkill : Skill
{
    public PassiveSkillEffect PassiveEffect { get; }
    public bool IsActive { get; }
}

public record PassiveSkillEffect
{
    public float MovementSpeedMultiplier { get; }
    public float ExperienceMultiplier { get; }
    public int HealthRegeneration { get; }
}
```

Skill ドメインは、プレイヤーの成長システムの重要な要素として、長期的なゲームプレイの動機づけとカスタマイゼーションを提供します。