# Character ドメイン

## 概要

Character ドメインは、プレイヤーの基本情報、レベル・経験値システム、戦闘能力、ステータス管理を担当する中核ドメインです。

## ドメインモデル

### エンティティ

#### PlayerCommonEntity
プレイヤーの基本情報を管理するルートエンティティ。

```csharp
public record PlayerCommonEntity
{
    public PlayerID Id { get; }  // 【未実装】実装では CharacterID を使用
    public PlayerName Name { get; }
    public PlayerLevel Level { get; }
    public Experience Experience { get; }
    public Strength Strength { get; }  // 【未実装】
    public Agility Agility { get; }  // 【未実装】
    public StatusPoint StatusPoint { get; }  // 【未実装】
    public ImmutableArray<SkillAndProficiency> Skills { get; }  // 【未実装】
}
```

#### PlayerBattleEntity
戦闘時の状態を管理する戦闘特化エンティティ。

```csharp
public record PlayerBattleEntity  // 【未実装】
{
    public PlayerID Id { get; }  // 【未実装】
    public PlayerName Name { get; }  // 【未実装】
    public CurrentHealth CurrentHealth { get; }  // 【未実装】
    public CharacterBattleModule BattleModule { get; }  // 【未実装】
}
```

### 値オブジェクト

#### 基本パラメータ
```csharp
public record PlayerID : BasicID  // 【未実装】実装では CharacterID
{
    protected override string Prefix => "PLY_";
}

public record PlayerName  // 【部分実装】バリデーションロジック未実装
{
    public string Value { get; }
    // バリデーション: 3-20文字
}

public record PlayerLevel  // 【部分実装】バリデーションロジック未実装
{
    public int Value { get; }
    // バリデーション: 1以上
}
```

#### 能力値システム
```csharp
public record Strength  // 【未実装】
{
    public int Value { get; }
    // 筋力: 物理攻撃・防御に影響
}

public record Agility  // 【未実装】
{
    public int Value { get; }
    // 俊敏性: 物理攻撃・防御に影響
}

public record StatusPoint  // 【未実装】
{
    public int Value { get; }
    // レベルアップ時に3ポイント獲得
}
```

#### 経験値システム
```csharp
public record Experience
{
    public int Value { get; }
    
    // 指数関数的レベル計算
    private static readonly int InitExpRequirement = 100;
    private static readonly float LevelDifferenceMultiplier = 1.25f;
}
```

#### 戦闘パラメータ
```csharp
public record BattleParameter  // 【未実装】
{
    public int MaxHealth { get; }      // レベル × 10
    public int AttackValue { get; }    // (STR + AGI) × 2 + 装備補正
    public int DefenseValue { get; }   // (STR + AGI) × 2 + 装備補正
}

public record CurrentHealth  // 【未実装】
{
    public int Value { get; }
    
    public CurrentHealth TakeDamage(int damage) => 
        new(Math.Max(0, Value - Math.Max(1, damage)));
}
```

### ファクトリー・コマンド

#### PlayerFactory
```csharp
public static class PlayerFactory  // 【未実装】
{
    public static PlayerCommonEntity CreateNew(PlayerCommonCreateCommand command);
    public static PlayerCommonEntity Reconstruct(PlayerCommonReconstructCommand command);
    public static PlayerBattleEntity CreateBattleEntity(PlayerCommonEntity common);
}
```

#### コマンドオブジェクト
```csharp
public record PlayerCommonCreateCommand  // 【未実装】
{
    public PlayerName Name { get; }
    public PlayerLevel Level { get; }
    public Strength Strength { get; }  // 【未実装】
    public Agility Agility { get; }  // 【未実装】
}

public record PlayerCommonReconstructCommand  // 【未実装】
{
    public PlayerID Id { get; }  // 【未実装】
    public PlayerName Name { get; }
    public PlayerLevel Level { get; }
    public Experience Experience { get; }
    public Strength Strength { get; }  // 【未実装】
    public Agility Agility { get; }  // 【未実装】
    public StatusPoint StatusPoint { get; }  // 【未実装】
    public ImmutableArray<SkillAndProficiency> Skills { get; }  // 【未実装】
}
```

## ビジネスルール

### レベル・経験値システム

#### 基本ルール
- **最低レベル**: 1
- **経験値下限**: 0
- **レベルアップ条件**: 必要経験値に到達

#### 経験値計算式
```csharp
// 指数関数的成長システム
// 必要経験値 = I * (1 - A^(level-1)) / (1 - A)
// I: 初期値 = 100, A: 倍率 = 1.25

public int GetRequiredExperience(int level)
{
    if (level <= 1) return 0;
    
    var multiplier = LevelDifferenceMultiplier;
    var numerator = InitExpRequirement * (1 - Math.Pow(multiplier, level - 1));
    var denominator = 1 - multiplier;
    
    return (int)(numerator / denominator);
}
```

#### レベル別必要経験値例
| レベル | 累計必要経験値 | 差分経験値 |
|--------|----------------|------------|
| 1      | 0              | -          |
| 2      | 100            | 100        |
| 3      | 225            | 125        |
| 5      | 576            | 195        |
| 10     | 2,441          | 488        |
| 20     | 47,684         | 2,384      |

### ステータスシステム

#### 基本能力値制約
- **Strength/Agility最低値**: 1
- **初期値**: 各1
- **レベルアップボーナス**: 3ポイント

#### ステータス配分ルール
```csharp
public PlayerCommonEntity AllocateStatus(int strIncrease, int agiIncrease)
{
    // 消費ポイント制限
    if (strIncrease + agiIncrease > StatusPoint.Value)
        throw new InvalidOperationException("Insufficient status points");
    
    // 最低値制限
    if (Strength.Value + strIncrease < 1 || Agility.Value + agiIncrease < 1)
        throw new InvalidOperationException("Status cannot be less than 1");
    
    return this with
    {
        Strength = new Strength(Strength.Value + strIncrease),
        Agility = new Agility(Agility.Value + agiIncrease),
        StatusPoint = new StatusPoint(StatusPoint.Value - strIncrease - agiIncrease)
    };
}
```

### 体力システム

#### 最大体力計算
```csharp
public int GetMaxHealth(PlayerLevel level) => level.Value * 10;
```

#### 体力変動ルール
```csharp
public record CurrentHealth
{
    // ダメージ処理（最低1ダメージ保証）
    public CurrentHealth TakeDamage(int damage)
    {
        var actualDamage = Math.Max(1, damage);
        return new CurrentHealth(Math.Max(0, Value - actualDamage));
    }
    
    // 回復処理（最大値制限）
    public CurrentHealth Heal(int amount, int maxHealth)
    {
        return new CurrentHealth(Math.Min(maxHealth, Value + amount));
    }
}
```

### 戦闘能力計算

#### 基本攻撃力・防御力
```csharp
// レベル依存攻撃力/防御力
public int GetLevelDependentAttack() => (Strength.Value + Agility.Value) * 2;
public int GetLevelDependentDefense() => (Strength.Value + Agility.Value) * 2;
```

**注意**: 装備補正やステータス効果を含む総合的な戦闘能力計算は [BattleSystem.md](../systems/BattleSystem.md) を参照してください。

## ゲームロジック

### プレイヤー作成フロー

```csharp
// 1. 新規プレイヤー作成
var createCommand = new PlayerCommonCreateCommand
{
    Name = new PlayerName("冒険者"),
    Level = new PlayerLevel(1),
    Strength = new Strength(1),
    Agility = new Agility(1)
};

var player = PlayerFactory.CreateNew(createCommand);

// 2. 戦闘エンティティ生成
var battleEntity = PlayerFactory.CreateBattleEntity(player);
```

### レベルアップ処理

```csharp
public PlayerCommonEntity ProcessLevelUp(int gainedExperience)
{
    var newExperience = new Experience(Experience.Value + gainedExperience);
    var currentLevel = Level.Value;
    
    // レベルアップ判定
    while (newExperience.Value >= GetRequiredExperience(currentLevel + 1))
    {
        currentLevel++;
        var newStatusPoint = StatusPoint.Value + 3; // レベルアップボーナス
        
        return this with
        {
            Level = new PlayerLevel(currentLevel),
            Experience = newExperience,
            StatusPoint = new StatusPoint(newStatusPoint)
        };
    }
    
    // レベルアップなしの場合
    return this with { Experience = newExperience };
}
```

### スキル習得管理

```csharp
public PlayerCommonEntity LearnSkill(Skill skill, SkillProficiency initialProficiency)
{
    // 既存スキルチェック
    if (Skills.Any(s => s.Skill.Name.Value == skill.Name.Value))
        throw new InvalidOperationException("Skill already learned");
    
    // 習得条件チェック（詳細は [Skill.md](./Skill.md) 参照）
    if (!CanLearnSkill(skill))
        throw new InvalidOperationException("Skill requirements not met");
    
    var newSkill = new SkillAndProficiency(skill, initialProficiency);
    return this with 
    { 
        Skills = Skills.Add(newSkill) 
    };
}
```

## 他ドメインとの連携

### Item ドメインとの連携
- **装備効果**: 戦闘パラメータ計算時に装備補正を適用
- **装備条件**: レベル・ステータス要件チェック
- 詳細: [EquipmentSystem.md](../systems/EquipmentSystem.md)

### Skill ドメインとの連携
- **スキル習得**: レベル・ステータス条件チェック
- **スキル熟練度**: 戦闘・生産活動による成長
- 詳細: [Skill.md](./Skill.md)

### PassiveEffect ドメインとの連携
- **ステータス効果**: 戦闘パラメータへの補正適用
- **効果持続**: ターン制管理
- 詳細: [BattleSystem.md](../systems/BattleSystem.md)

## 拡張ポイント

### 新ステータス追加
```csharp
// 将来的な拡張例：知力（Intelligence）の追加
public record Intelligence
{
    public int Value { get; }
}

// PlayerCommonEntityへの追加
public record PlayerCommonEntity
{
    // 既存プロパティ...
    public Intelligence Intelligence { get; }
}
```

### レベル上限設定
```csharp
public record PlayerLevel
{
    public const int MaxLevel = 100;
    
    public PlayerLevel(int value)
    {
        if (value < 1 || value > MaxLevel)
            throw new ArgumentOutOfRangeException();
        Value = value;
    }
}
```

Character ドメインは他の多くのドメインとの統合点となる中核的な役割を担っています。