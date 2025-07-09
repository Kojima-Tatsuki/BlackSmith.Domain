# Skill ドメイン

## 概要

Skill ドメインは、プレイヤーのスキルシステムの基本構造を提供します。\
現在の実装では、スキルの抽象クラス、インターフェース、経験値管理システム、取得条件管理が実装されています。\
戦闘スキルと生産スキルのインターフェースは定義されていますが、具体的な実装や効果システムは将来的な拡張です。

## ドメインモデル

### 基底クラス・インターフェース

#### Skill（抽象基底クラス）
```csharp
// 【実装済み】スキル抽象基底クラス
public abstract class Skill : ISkill
{
    public SkillID ID { get; }
    public SkillName SkillName { get; }
    public SkillProficiency Proficiency { get; }
    public SkillAcquisitionConditions AcquisitionConditions { get; }
    
    internal Skill(SkillID id, SkillName skillName, SkillExperience exp, SkillAcquisitionConditions acquisitionConditions)
    {
        ID = id ?? throw new ArgumentNullException(nameof(id));
        SkillName = skillName ?? throw new ArgumentNullException(nameof(skillName));
        Proficiency = new SkillProficiency(exp) ?? throw new ArgumentNullException(nameof(exp));
        AcquisitionConditions = acquisitionConditions ?? throw new ArgumentNullException(nameof(acquisitionConditions));
    }
    
    internal bool CanSkillAcquisition(SkillAcquisitionConditions requireParaeters) => AcquisitionConditions.CanSkillAcquisition(requireParaeters);
}

// 【実装済み】スキルID（BasicIDベース）
public record SkillID : BasicID
{
    protected override string Prefix => "Skill-";
}
```

### スキルインターフェース

#### ISkill（基本インターフェース）
```csharp
// 【実装済み】スキル基本インターフェース
internal interface ISkill
{
    SkillName SkillName { get; }
    SkillProficiency Proficiency { get; }
    SkillAcquisitionConditions AcquisitionConditions { get; }
}
```

#### IProductionSkill（生産スキル）
```csharp
// 【実装済み】生産スキルインターフェース
internal interface IProductionSkill : ISkill
{
    CreateSkillAction CreateAction { get; }
    RefineSkillAction RefineAction { get; }
    RepairSkillAction RepairAction { get; }
}
```

#### IBattleSkill（戦闘スキル）
```csharp
// 【部分実装】戦闘スキルインターフェース（内容は要検討）
internal interface IBattleSkill : ISkill
{
    // 戦闘スキルの詳細な実装は未定
}
```

> **注意**: 戦闘スキルの具体的な実装については検討中です。

### 値オブジェクト

#### SkillName
```csharp
// 【実装済み】スキル名の値オブジェクト
public class SkillName
{
    public string Value { get; }
    
    internal SkillName(string value)
    {
        if (!IsValid(value))
            throw new AggregateException(nameof(value));
        
        Value = value;
    }
    
    internal static bool IsValid(string value)
    {
        if (value is null)
            return false;
        
        return true;
    }
}
```

#### SkillProficiency（熟練度）
```csharp
// 【実装済み】スキル熟練度管理クラス（経験値ベース）
public class SkillProficiency
{
    /// <summary>熟練度 1-1000</summary>
    public int Value { get; }
    
    /// <summary>累計獲得経験値</summary>
    public SkillExperience CumulativeExp { get; }
    
    internal SkillProficiency(SkillExperience exp = null!)
    {
        var calclator = new SkillExpCalculator();
        
        CumulativeExp = exp ?? new SkillExperience();
        Value = calclator.CurrentProficiency(CumulativeExp);
    }
    
    /// <summary>経験値を加算する</summary>
    internal SkillProficiency AddExp(SkillExperience exp)
    {
        return new SkillProficiency(CumulativeExp.Add(exp));
    }
}
```

#### SkillExperience（スキル経験値）
```csharp
// 【実装済み】スキル経験値管理クラス
public class SkillExperience
{
    public int Value { get; }
    
    internal SkillExperience(int value = 0)
    {
        if (!IsValid(value))
            throw new ArgumentOutOfRangeException(nameof(value));
        
        Value = value;
    }
    
    internal static bool IsValid(int value)
    {
        if (value < 0)
            return false;
        
        return true;
    }
    
    internal SkillExperience Add(SkillExperience other)
    {
        return new SkillExperience(Value + other.Value);
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
    
    internal SkillAndProficiency(Skill skill, SkillProficiency proficiency)
    {
        Skill = skill;
        Proficiency = proficiency;
    }
}
```

### 習得条件

#### SkillAcquisitionConditions
```csharp
// 【実装済み】スキル取得条件クラス
public class SkillAcquisitionConditions
{
    public PlayerLevel Level { get; }
    public Strength Strength { get; }
    public Agility Agility { get; }
    public IReadOnlyCollection<SkillAndProficiency> RequiredSkills { get; }
    
    internal SkillAcquisitionConditions(PlayerLevel level, Strength strength, Agility agility, IReadOnlyCollection<SkillAndProficiency>? requireSkills = null)
    {
        Level = level;
        Strength = strength;
        Agility = agility;
        RequiredSkills = requireSkills ?? new List<SkillAndProficiency>();
    }
    
    internal static SkillAcquisitionConditions FromDependentParams(LevelDependentParameters parameters)
    {
        return new SkillAcquisitionConditions(parameters.Level, parameters.STR, parameters.AGI);
    }
    
    internal bool CanSkillAcquisition(SkillAcquisitionConditions condition)
    {
        if (condition.Level.Value < Level.Value)
            return false;
        
        return true;
    }
}
```

## ビジネスルール

### スキル習得条件

#### 基本条件チェック
```csharp
// 【実装済み】Skillクラス内での基本的な条件チェック
public abstract class Skill : ISkill
{
    // ...
    
    /// <summary>スキルが取得できるか</summary>
    internal bool CanSkillAcquisition(SkillAcquisitionConditions requireParaeters) => 
        AcquisitionConditions.CanSkillAcquisition(requireParaeters);
}

// 【実装済み】SkillAcquisitionConditions内での基本チェック
public class SkillAcquisitionConditions
{
    // ...
    
    internal bool CanSkillAcquisition(SkillAcquisitionConditions condition)
    {
        if (condition.Level.Value < Level.Value)
            return false;
        
        return true;
    }
}
```

> **注意**: 現在の実装ではレベル条件のみチェックしています。ステータスや前提スキルの詳細チェックは将来的な拡張です。

### 熟練度システム

#### 経験値計算システム
```csharp
// 【実装済み】スキル経験値計算機
internal class SkillExpCalculator
{
    // 1Lv -> 2Lv になるために行う動作の回数
    private readonly int InitRequireCount = 15;
    
    // 1Lv -> 2Lv になるために必要な経験値
    private readonly int InitRequireExp = 100;
    
    // レベル差倍率
    private readonly float LevelDifferenceMultiplier = 1.1f;
    
    /// <summary>次のレベルまでに必要な経験値</summary>
    internal SkillExperience NeedToNextLevel(int level)
    {
        return new SkillExperience((int)(InitRequireExp * Math.Pow(LevelDifferenceMultiplier, level - 1)));
    }
    
    /// <summary>1動作あたりの取得経験値量</summary>
    internal SkillExperience ReceveExp(int level)
    {
        return new SkillExperience((int)Math.Round(InitRequireExp / InitRequireCount * Math.Pow(LevelDifferenceMultiplier, level - 1)));
    }
    
    /// <summary>累計獲得経験値から現在の熟練度を算出する</summary>
    internal int CurrentProficiency(SkillExperience cumExp)
    {
        return (int)Math.Log(1 - (cumExp.Value / InitRequireExp * (1 - LevelDifferenceMultiplier)), LevelDifferenceMultiplier) + 1;
    }
}
```

#### 経験値加算システム
```csharp
// 【実装済み】SkillProficiency内での経験値加算
public class SkillProficiency
{
    // ...
    
    /// <summary>経験値を加算する</summary>
    internal SkillProficiency AddExp(SkillExperience exp)
    {
        return new SkillProficiency(CumulativeExp.Add(exp));
    }
}
```

### スキルアクションシステム

#### 生産スキルアクション
```csharp
// 【部分実装】生産スキルアクションインターフェースのみ定義
internal interface IProductionSkill : ISkill
{
    CreateSkillAction CreateAction { get; }
    RefineSkillAction RefineAction { get; }
    RepairSkillAction RepairAction { get; }
}
```

> **注意**: `CreateSkillAction`, `RefineSkillAction`, `RepairSkillAction` の具体的な実装は未定です。

#### 戦闘スキルシステム
```csharp
// 【部分実装】戦闘スキルインターフェースのみ定義
internal interface IBattleSkill : ISkill
{
    // 戦闘スキルの内容については要検討
}
```

> **注意**: 戦闘スキルの具体的な効果や実装は検討中です。

## ゲームロジック

### Characterドメインとの統合

#### スキル数制限システム
```csharp
// 【実装済み】CharacterLevel内でのスキル数管理
public record CharacterLevel
{
    public int Value { get; }
    
    /// <summary>取得できるスキル数を返す</summary>
    internal int GetNumberOfSkillsAvailable()
    {
        // 初期状態で2つ
        // 6, 12レベルで1つずつ増える
        // 以降、10レベル毎で1つ増える
        
        if (Value < 12)
            return 2 + (int)Math.Floor((double)Value / 6);
        
        return 3 + (int)Math.Floor((double)Value / 10);
    }
}
```

> **注意**: スキルの具体的な習得・成長システムは未実装です。キャラクターレベルによるスキル数制限のみ実装されています。

### スキルシステム統合

```csharp
// 【部分実装】スキルシステムの基本構造のみ実装済み

// 1. スキルインターフェース階層
internal interface ISkill { /* ... */ }
internal interface IProductionSkill : ISkill { /* ... */ }
internal interface IBattleSkill : ISkill { /* ... */ }

// 2. スキル実装基底クラス
public abstract class Skill : ISkill { /* ... */ }

// 3. スキル経験値管理システム
public class SkillExperience { /* ... */ }
public class SkillProficiency { /* ... */ }
internal class SkillExpCalculator { /* ... */ }

// 4. スキル取得条件システム
public class SkillAcquisitionConditions { /* ... */ }
```

> **注意**: スキルの具体的な習得・使用・効果システムは未実装です。現在はスキルシステムの基本的なデータ構造と経験値計算のみが実装されています。



## 拡張ポイント

現在の実装はスキルシステムの基本的なデータ構造と経験値管理のみです。以下の機能が将来的な拡張ポイントとして考えられます：

### スキル実装系
- **具体的なスキルクラス**: `Skill` 抽象クラスを継承した実際のスキル実装
- **戦闐スキル効果**: `IBattleSkill` インターフェースの具体化
- **生産スキルアクション**: `CreateSkillAction`, `RefineSkillAction`, `RepairSkillAction`

### スキル管理系
- **スキル習得システム**: キャラクターがスキルを習得する機能
- **スキル使用システム**: 習得したスキルを実際に使用する機能
- **スキル成長システム**: 経験値獲得による熟練度上昇

### 高度なスキル機能
- **スキルツリーシステム**: スキル間の依存関係管理
- **スキルコンボシステム**: 複数スキルの組み合わせ効果
- **パッシブスキルシステム**: 常時発動する補助効果

