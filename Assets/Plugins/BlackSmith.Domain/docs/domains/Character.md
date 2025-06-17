# Character ドメイン

## 概要

Character ドメインは、プレイヤーのアイデンティティと戦闘機能を管理する中核ドメインです。

### 主要な責務
- **基本情報管理**：プレイヤーの名前、レベル、経験値の永続的な管理
- **戦闘情報管理**：HP、装備、ステータス効果などの戦闘時状態管理
- **レベルシステム**：指数関数的経験値成長と累計経験値からの自動レベル計算
- **ステータスシステム**：レベル依存パラメータ（STR/AGI）の配分管理
- **戦闘能力計算**：レベル、装備、ステータス効果を統合した攻撃力・防御力算出

### 設計の特徴
- **責務分離設計**：`PlayerCommonEntity`（基本情報）と`PlayerBattleEntity`（戦闘情報）の明確な分離
- **DDD原則**：値オブジェクト、エンティティ、ファクトリー、リポジトリパターンの適用
- **自動管理**：経験値からのレベル計算、ステータスポイント管理の自動化
- **拡張性**：将来的な機能追加に対応する柔軟なアーキテクチャ

このドメインは他の多くのドメインとの統合点となる基盤的な役割を担っています。

## ドメインモデル

### エンティティ

#### PlayerCommonEntity

**設計思想**：
`PlayerCommonEntity`はプレイヤーの基本的なアイデンティティ情報を管理するエンティティです。戦闘に関連しない永続的な情報のみを扱い、プレイヤーの本質的な属性（名前、レベル、経験値）を責務として持ちます。

プレイヤーの基本情報を管理するルートエンティティ。`ICharacterEntity`と`IEquatable<PlayerCommonEntity>`を実装し、プレイヤーの基本的な操作を提供します。

```csharp
public class PlayerCommonEntity : ICharacterEntity, IEquatable<PlayerCommonEntity>
{
    public CharacterID ID { get; }
    public PlayerName Name { get; private set; }
    public PlayerLevel Level { get; private set; }
    
    // ICharacterEntityインターフェース実装
    CharacterLevel ICharacterEntity.Level => Level;
    
    // 名前変更機能（ICharacterEntityで実装）
    void ICharacterEntity.ChangeName(PlayerName name);
    
    // 再構築コマンド取得
    public PlayerCommonReconstructCommand GetReconstractCommand();
    
    // ID基準の等価判定
    public bool Equals(PlayerCommonEntity other);
}
```

**特徴**：
- **基本情報のみ管理**：戦闘情報（HP、装備等）は含まない
- **名前変更機能**：インターフェース経由での名前変更をサポート
- **レベル管理**：`PlayerLevel`で累計経験値からの自動レベル計算
- **ID基準等価性**：同一IDであれば同一プレイヤーとして判定

#### PlayerBattleEntity

**設計思想**：
`PlayerBattleEntity`は戦闘時に使用する情報を管理するエンティティで、`PlayerCommonEntity`（基本情報）から責務を分離しています。一般的なRPGのように「一つのプレイヤーエンティティが全てを管理する」設計ではなく、以下の理由で情報を分離しています：

- **関心の分離**：基本情報（名前・レベル・経験値）と戦闘情報（HP・装備・ステータス効果）の責務を明確化
- **データモデルの単純化**：各エンティティが自分の関心領域のみを扱うことで理解しやすさ向上
- **保守性の向上**：基本情報と戦闘ロジックの変更が相互に影響しない独立性
- **システムの柔軟性**：将来的な機能拡張時に各エンティティを独立して変更可能

この設計により、戦闘システムと基本情報管理がそれぞれの責務に集中でき、システム全体の可読性と保守性が向上します。

戦闘時の状態を管理する戦闘特化エンティティ。`IBattleCharacter`と`ITakeDamageable`を実装し、戦闘に必要な機能を提供します。

```csharp
public class PlayerBattleEntity : IBattleCharacter, IEquatable<PlayerBattleEntity>
{
    public CharacterID ID { get; }
    internal CharacterBattleModule BattleModule { get; set; }
    
    // 戦闘情報へのアクセサー
    public CharacterLevel Level => BattleModule.Level;
    public HealthPoint HealthPoint => BattleModule.HealthPoint;
    public AttackValue Attack => BattleModule.GetAttack();
    public DefenseValue Defense => BattleModule.GetDefense();
    
    // 戦闘アクション（ITakeDamageableで実装）
    HealthPoint ITakeDamageable.TakeDamage(DamageValue damage);
    HealthPoint ITakeDamageable.HealHealth(int value);
    
    // 装備管理
    internal ChangeBattleEquipmentResult ChangeBattleEquipment(EquippableItem item);
    internal ChangeBattleEquipmentResult RemoveBattleEquipment(EquipmentType removeType);
    
    // 再構築コマンド取得
    public PlayerBattleReconstructCommand GetReconstractCommand();
}
```

### 値オブジェクト

#### 基本パラメータ
```csharp
public record CharacterID : BasicID
{
    protected override string Prefix => "Character-";
}

public record PlayerName
{
    public string Value { get; }
    // バリデーション: 1文字以上
}

public record PlayerLevel : CharacterLevel
{
    public int MaxValue { get; } = 100;
    public Experience CumulativeExp { get; }
    // 累計経験値による自動レベル計算
}
```

#### レベル依存パラメータ

**設計思想**：
`LevelDependentParameters`は、レベルとそのレベルに依存するステータス（STR/AGI）を一体で管理する値オブジェクトです。一般的なRPGのように「キャラクターがレベルとSTR/AGIを個別に持つ」設計ではなく、「レベルが決まると利用可能なステータスポイント総量が決まり、そのポイント配分結果としてSTR/AGIが決まる」という設計になっています。

この設計により以下のメリットを実現：
- **整合性保証**：レベルとステータスの不整合を防止
- **不変性**：record型による値オブジェクトとして状態変更を制御
- **責務の明確化**：レベル依存パラメータの計算ロジックを一箇所に集約

```csharp
public record LevelDependentParameters
{
    public PlayerLevel Level { get; }
    public Strength STR { get; }
    public Agility AGI { get; }
    
    // レベル × 3 のステータス上昇ポイント管理
    private const int ReceivedPointsPerLevelIncrease = 3;
    
    public int GetRemainingParamPoint();
    public LevelDependentParameters AddParamPoint(int str, int agi);
    public static int GetLevelDependParamPoint(PlayerLevel level);
}

public record Strength
{
    public int Value { get; }
    // BasePlayerParameter使用（1以上のバリデーション）
}

public record Agility
{
    public int Value { get; }
    // BasePlayerParameter使用（1以上のバリデーション）
}

internal record BasePlayerParameter
{
    public int Value { get; }
    // 1以上のバリデーション機能
}
```

#### 経験値システム

**設計思想**：
Experienceクラスは、指数関数的成長でレベルアップに必要な経験値が增加するシステムです。敵を一定数倒すことでレベルアップし、累計経験値から現在レベルを逆算する機能も提供します。

```csharp
public record Experience
{
    public int Value { get; }
    
    // 指数関数的レベル計算の定数
    private static readonly int InitKillRequirement = 5;        // 1→2Lvに必要な敵数
    private static readonly int InitExpRequirement = 100;       // 1→2Lvに必要な経験値
    private static readonly float LevelDifferenceMultiplier = 1.25f;  // レベル差倍率
    
    // 経験値操作
    public Experience Add(Experience other);
    
    // レベル計算関連
    public static int CurrentLevel(Experience cumExp);                    // 累計経験値からレベル算出
    public static Experience RequiredCumulativeExp(int level);            // レベル到達に必要な累計経験値
    public static Experience NeedToNextLevel(int level);                 // 次レベルまでの必要経験値
    public static Experience ReceveExp(int enemyLevel);                   // 敵レベルに応じた取得経験値
}
```

**経験値計算式**：
```
// 累計必要経験値 = I * (1 - A^(level-1)) / (1 - A)
// I: InitExpRequirement = 100
// A: LevelDifferenceMultiplier = 1.25

// 敵一体あたりの取得経験値 = 次レベル必要経験値 / 必要敵数(5体)
```

#### 戦闘パラメータ

##### CharacterBattleModule
戦闘時のキャラクター情報を統合管理するモジュール。

```csharp
public record CharacterBattleModule
{
    public CharacterLevel Level { get; }
    public HealthPoint HealthPoint { get; }
    public LevelDependentParameters LevelDependentParameters { get; }
    public BattleEquipmentModule EquipmentModule { get; }
    public BattleStatusEffectModule StatusEffectModule { get; }
    
    // 戦闘能力計算（シリアライズ対象外）
    public AttackValue GetAttack();
    public DefenseValue GetDefense();
    
    // 戦闘アクション
    internal CharacterBattleModule TakeDamage(DamageValue damage);
    internal CharacterBattleModule HealHealth(int value);
    internal ChangeEquipmentResult ChangeBattleEquipment(EquippableItem? item, EquipmentType changeType);
}
```

##### HealthPoint
体力管理クラス。

```csharp
public record HealthPoint
{
    public int CurrentValue { get; }
    public int MaxValue { get; }
    
    public HealthPoint TakeDamage(DamageValue damage);
    public HealthPoint HealHealth(int amount);
}
```

##### 戦闘能力関連
```csharp
public record AttackValue
{
    public int Value { get; }
    // レベル、装備、ステータス効果から自動計算
}

public record DefenseValue
{
    public int Value { get; }
    // レベル、装備、ステータス効果から自動計算
}

public record DamageValue
{
    public int Value { get; }
    // レベル差、攻撃力、防御力から計算
}
```

### ファクトリー・コマンド

#### PlayerFactory
```csharp
public static class PlayerFactory
{
    public static PlayerCommonEntity Reconstruct(PlayerCommonReconstructCommand command);
}
```

#### コマンドオブジェクト
```csharp
public record PlayerCommonReconstructCommand
{
    public CharacterID Id { get; }
    public PlayerName Name { get; }
    public PlayerLevel Level { get; }
    public LevelDependentParameters LevelDependentParameters { get; }
}

public record PlayerBattleReconstructCommand
{
    public CharacterID Id { get; }
    public BattleCharacterModule BattleModule { get; }
}
```

### ユースケース

#### プレイヤー管理
```csharp
public class AdjustPlayerCommonEntityUsecase
{
    public UniTask<PlayerCommonEntity> CreatePlayerAsync(PlayerName name);
    public UniTask<PlayerCommonEntity> GetPlayerAsync(CharacterID id);
    public UniTask DeletePlayerAsync(CharacterID id);
}

public class AdjustPlayerBattleEntityUsecase
{
    public UniTask<PlayerBattleEntity> CreateBattleEntityAsync(CharacterID id);
    public UniTask<PlayerBattleEntity> GetBattleEntityAsync(CharacterID id);
    public UniTask DeleteBattleEntityAsync(CharacterID id);
}
```

#### 戦闘関連
```csharp
public class CharacterDamageUsecase
{
    public UniTask<DamageValue> CalculateDamageAsync(CharacterID attackerId, CharacterID defenderId);
}

public class PlayerBattleEntityProvideUsecase
{
    public static PlayerBattleEntity BuildBattleEntity(PlayerBattleReconstructCommand command);
}
```

## ビジネスルール

### レベル・経験値システム

#### 基本ルール
- **最低レベル**: 1
- **経験値下限**: 0
- **レベルアップ条件**: 累計経験値が必要量に到達

#### レベル別必要経験値例（実装連動）
| レベル | 累計必要経験値 | 差分経験値 |
|--------|----------------|------------|
| 1      | 0              | -          |
| 2      | 100            | 100        |
| 3      | 225            | 125        |
| 5      | 576            | 351        |
| 10     | 2,580          | 2,004      |
| 20     | 27,355         | 24,775     |

### ステータスシステム

#### 基本能力値制約
- **Strength/Agility最低値**: 1 (BasePlayerParameterでバリデーション)
- **初期値**: 各1
- **レベルアップボーナス**: 3ポイント (ReceivedPointsPerLevelIncrease)

#### ステータス配分ルール
ステータスの配分は`LevelDependentParameters`で管理されます。

```csharp
public LevelDependentParameters AddParamPoint(int str, int agi)
{
    // 消費ポイント制限チェック
    if (GetRemainingParamPoint() < str + agi)
        throw new ArgumentException($"指定された上昇量が加算できる値を超えています");

    // 新しいステータスでLevelDependentParametersを作成
    return new LevelDependentParameters(
        Level,
        new Strength(STR.Value + str),
        new Agility(AGI.Value + agi));
}

// 使用可能ステータスポイント計算
public int GetRemainingParamPoint()
{
    return Level.Value * 3 - (STR.Value + AGI.Value);
}
```

### 体力システム

体力管理は`HealthPoint`クラスで実装されています。

```csharp
public record HealthPoint
{
    public int CurrentValue { get; }
    public int MaxValue { get; }
    
    // ダメージ処理
    public HealthPoint TakeDamage(DamageValue damage);
    
    // 回復処理
    public HealthPoint Heal(int amount);
}
```

### 戦闘能力計算

戦闘能力は`AttackValue`と`DefenseValue`クラスで管理され、以下の4つの要素から総合的に計算されます。

#### 攻撃力計算 (AttackValue)
```csharp
internal AttackValue(LevelDependentParameters levelParams, 
                    BattleEquipmentModule? equipmentModule, 
                    BattleStatusEffectModule? statusEffectModule)
{
    // 1. レベル依存攻撃力: (STR + AGI) × 2
    FromLevelAttack = (levelParams.STR.Value + levelParams.AGI.Value) * 2;
    
    // 2. 武器攻撃力
    FromWeaponAttack = equipmentModule?.Weapon?.Attack?.Value ?? 0;
    
    // 3. 防具攻撃力
    FromArmorAttack = equipmentModule?.Armor?.Attack?.Value ?? 0;
    
    // 4. ステータス効果攻撃力
    FromStatusEffectAttack = statusEffectModule?.StatusEffects.Sum(effect => effect.StatusModel.Attack) ?? 0;
    
    // 総合攻撃力 = 全ての合計（1以上保証）
    Value = FromLevelAttack + FromWeaponAttack + FromArmorAttack + FromStatusEffectAttack;
}
```

#### 防御力計算 (DefenseValue)
```csharp
internal DefenseValue(LevelDependentParameters levelParams, 
                     BattleEquipmentModule? equipmentModule, 
                     BattleStatusEffectModule? statusEffectModule)
{
    // 1. レベル依存防御力: (STR + AGI) × 2
    FromLevelDefence = (levelParams.STR.Value + levelParams.AGI.Value) * 2;
    
    // 2. 武器防御力
    FromWeaponDefense = equipmentModule?.Weapon?.Defense?.Value ?? 0;
    
    // 3. 防具防御力
    FromArmorDefense = equipmentModule?.Armor?.Defense?.Value ?? 0;
    
    // 4. ステータス効果防御力
    FromStatusEffectDefense = statusEffectModule?.StatusEffects.Sum(effect => effect.StatusModel.Defense) ?? 0;
    
    // 総合防御力 = 全ての合計（1以上保証）
    Value = FromLevelDefence + FromWeaponDefense + FromArmorDefense + FromStatusEffectDefense;
}
```

**特徴**：
- 攻撃力・防御力は同じ計算式（STR + AGI）× 2
- 装備やステータス効果を自動的に統合
- 最終値は1以上が保証される

## ゲームロジック

### プレイヤー作成フロー

#### 1. 新規プレイヤー作成
```csharp
// ユースケースの初期化（リポジトリが必要）
var repository = // IPlayerCommonEntityRepositoryの実装
var usecase = new AdjustPlayerCommonEntityUsecase(repository);

// プレイヤー作成（初期レベル1、経験値 0で作成）
var player = await usecase.CreateCharacter("冒険者");
```

#### 2. プレイヤー再構築
```csharp
// 既存データからプレイヤーを再構築
var command = new PlayerCommonReconstructCommand(
    characterId,
    new PlayerName("冒険者"),
    new PlayerLevel(cumulativeExp) // 累計経験値からレベル自動計算
);

var player = await usecase.ReconstructPlayer(command);
```

#### 3. プレイヤー情報取得・削除
```csharp
// プレイヤー取得
var player = await usecase.GetCharacter(characterId);
if (player == null) {
    // プレイヤーが存在しない
}

// プレイヤー削除
await usecase.DeletePlayer(characterId);
```

#### 4. 戦闘エンティティ作成（必要時）
```csharp
// 戦闘用エンティティは別ユースケースで管理
var battleRepository = // IBattleCharacterRepositoryの実装
var battleUsecase = new AdjustPlayerBattleEntityUsecase(battleRepository);
var battleEntity = await battleUsecase.CreateBattleEntityAsync(player.ID);
```

### 経験値とレベル管理

経験値とレベルは`PlayerLevel`クラスで自動管理されます。

```csharp
// 経験値の追加
var currentLevel = player.Level;
var newExperience = currentLevel.CumulativeExp.Add(new Experience(gainedExp));
var newLevel = new PlayerLevel(newExperience);

// レベルが上がったかチェック
if (newLevel.Value > currentLevel.Value) {
    // レベルアップ処理
    // LevelDependentParametersのステータスポイントが自動で増加
}

// 累計経験値からレベルを自動計算
var level = Experience.CurrentLevel(cumulativeExp);
```


## 他ドメインとの連携

### PlayerCommonEntity と PlayerBattleEntity の連携

この2つのエンティティは、同一プレイヤーを異なる観点で管理する分離設計を採用しています。

#### 連携方式
- **ID連携**: 同一`CharacterID`で同じプレイヤーを識別
- **データフロー**: `PlayerCommonEntity`から`PlayerBattleEntity`作成時に必要な情報を抽出
- **独立管理**: それぞれ独立したリポジトリとユースケースで管理

#### 責務分担
- **PlayerCommonEntity**: 
  - 基本情報（名前、レベル、経験値）
  - ゲーム進行に関わる永続的なデータ
  - プレイヤーのアイデンティティと成長管理
- **PlayerBattleEntity**: 
  - 戦闘関連情報（現在HP、装備、ステータス効果）
  - 戦闘時の状態管理と戦闘アクション処理
  - 装備変更やダメージ処理などの戦闘操作

#### 設計上の利点
- **関心の分離**: 基本情報と戦闘情報の独立管理
- **コードの可読性**: 各エンティティが自分の責務に集中
- **保守性の向上**: 基本情報と戦闘ロジックの独立変更
- **システムの柔軟性**: 将来的な機能拡張に対する柔軟性

### Item ドメインとの連携
> **⚠️ 要更新**: この情報は Item.md 修正後に実装済み機能との照合が必要です

- **装備効果**: 戦闘パラメータ計算時に装備補正を適用
- **装備条件**: レベル・ステータス要件チェック
- 詳細: [EquipmentSystem.md](../systems/EquipmentSystem.md)

### PassiveEffect ドメインとの連携
> **⚠️ 要更新**: この情報は PassiveEffect.md 修正後に実装済み機能との照合が必要です

- **ステータス効果**: 戦闘パラメータへの補正適用
- **効果持続**: ターン制管理
- 詳細: [BattleSystem.md](../systems/BattleSystem.md)

## まとめ

Character ドメインは、`PlayerCommonEntity`と`PlayerBattleEntity`の分離設計により、ゲームの基本情報管理と戦闘システムを独立させた中核的なドメインです。DDD の原則に従った値オブジェクト設計と、レベル・経験値・ステータスの自動管理システムにより、他ドメインとの統合点として安定した基盤を提供しています。