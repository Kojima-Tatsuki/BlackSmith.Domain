# PassiveEffect ドメイン

## 概要

PassiveEffect ドメインは、キャラクターに影響を与える一時的・持続的な効果を管理するドメインです。

### 主要な責務
- **戦闘ステータス効果**：攻撃力・防御力・体力・移動速度の補正管理
- **効果の識別**：EffectIDによるユニークな効果識別
- **効果の適用**：CharacterドメインのBattleStatusEffectModuleとの連携
- **継続効果**：時間ベースの効果処理システム

### 現在の実装状況
- **BattleStatusEffect**：基本的な戦闘ステータス効果を実装
- **EffectID**：GUID ベースの効果識別子を実装
- **BattleStatusEffectModel**：4種類のステータス補正（MaxHealth, Attack, Defense, MovementSpeed）を実装
- **ContinuouslyEffect**：継続効果インターフェースを実装
- **BattleStatusEffectModule**：Character ドメインでの効果管理を実装
- **PassiveEffectService**：継続効果処理サービスを実装

### 設計の特徴
- **イミュータブル設計**：record による不変オブジェクト設計
- **Character ドメイン連携**：BattleStatusEffectModule での効果コレクション管理
- **非同期処理対応**：UniTask による継続効果の非同期実行
- **JSON 対応**：完全なシリアライゼーション対応
- **ミニマル実装**：必要最小限の機能に絞った実装

このドメインは Character ドメインとの密接な連携により、戦闘システムの基盤効果管理を提供しています。

## ドメインモデル

### 識別子

#### 実装済み識別子

```csharp
// BattleStatusEffect.cs の実装済み識別子
public record EffectID
{
    public Guid Value { get; }
    
    [JsonConstructor]
    internal EffectID(Guid? value = null)
    {
        Value = value ?? Guid.NewGuid(); // 自動GUID生成
    }
}
```

**注意**：現在の実装では BasicID を継承していない独自の GUID ベース識別子です。

### エンティティ・値オブジェクト

#### 実装済みエンティティ

##### BattleStatusEffect (BattleStatusEffect.cs)
```csharp
// 実装済み：戦闘ステータス効果レコード
public record BattleStatusEffect
{
    public EffectID Id { get; }
    public BattleStatusEffectModel StatusModel { get; }
    
    [JsonConstructor]
    internal BattleStatusEffect(EffectID id, BattleStatusEffectModel statusModel);
}
```

##### BattleStatusEffectModel (BattleStatusEffect.cs)
```csharp
// 実装済み：ステータス補正値オブジェクト
public record BattleStatusEffectModel
{
    public int MaxHealth { get; }    // 体力補正
    public int Attack { get; }       // 攻撃力補正
    public int Defense { get; }      // 防御力補正
    public int MovementSpeed { get; } // 移動速度補正
    
    [JsonConstructor]
    internal BattleStatusEffectModel(int maxHealth, int attack, int defense, int movementSpeed);
}
```

#### 実装済みインターフェース

##### ContinuouslyEffect (ContinuouslyEffect.cs)
```csharp
// 実装済み：継続効果インターフェース
public interface ContinuouslyEffect
{
    float ContinueTime { get; }
    UniTask DoEffect(CharacterID targetId);
}
```

##### InstantEffect (ContinuouslyEffect.cs)
```csharp
// 実装済み：瞬間効果インターフェース（マーカーインターフェース）
public interface InstantEffect
{
    // 現在は空のマーカーインターフェース
}
```

#### 実装済みクラス

##### InstantDamageEffect (InstantDamageEvent.cs)
```csharp
// 実装済み（部分）：瞬間ダメージ効果（注意：実装は空）
internal class InstantDamageEffect : InstantEffect
{
    public void DoEffect(CharacterID id)
    {
        // 現在は空実装
    }
}
```

#### ドメインサービス

##### PassiveEffectService (PassiveEffectService.cs)
```csharp
// 実装済み：パッシブ効果処理サービス
internal class PassiveEffectService
{
    internal async UniTask PlayContinuouslyEffect(CharacterID targetId, ContinuouslyEffect effect)
    {
        await effect.DoEffect(targetId);
    }
}
```


## ビジネスルール

### 実装済みルール

#### 効果ID の一意性
```csharp
// BattleStatusEffectModule.cs の実装済み制限
internal BattleStatusEffectModule AddStatusEffect(BattleStatusEffect statusEffect)
{
    if (dict.Keys.Contains(statusEffect.Id))
        throw new InvalidOperationException($"The effect aleady exists. id: {statusEffect.Id}.");
    // 重複ID防止
}
```

#### 効果の削除制限
```csharp
// BattleStatusEffectModule.cs の実装済み制限
internal BattleStatusEffectModule RemoveStatusEffect(BattleStatusEffect statusEffect)
{
    if (!dict.Keys.Contains(statusEffect.Id))
        throw new InvalidOperationException($"Does not exist the effect. id: {statusEffect.Id}.");
    // 存在しない効果の削除エラー
}
```

#### GUID 自動生成
```csharp
// EffectID.cs の実装済み機能
internal EffectID(Guid? value = null)
{
    Value = value ?? Guid.NewGuid(); // 自動GUID生成
}
```

### 未実装ルール

**注意**：以下のビジネスルールはドキュメントに記載されていますが、現在未実装です：

#### 効果制限システム
- **最大効果数制限**：キャラクター当たりの最大効果数
- **同名効果制限**：同じ効果の重複制限
- **持続時間管理**：時間ベースの効果期限切れ

#### 効果重複ルール
- **スタッキング制御**：効果の重複許可・禁止制御
- **効果競合解決**：同じタイプの効果の優先度
- **効果上書き**：より強い効果による上書き

#### 効果種別分類
- **バフ・デバフ分類**：有利・不利効果の分類
- **効果タイプ**：戦闘・環境・永続等の分類

## ゲームロジック

### 実装済み機能

#### 効果作成と管理
```csharp
// BattleStatusEffect の基本使用例
var effectId = new EffectID();
var statusModel = new BattleStatusEffectModel(
    maxHealth: 50,    // +50 体力
    attack: 10,       // +10 攻撃力
    defense: 5,       // +5 防御力
    movementSpeed: 2  // +2 移動速度
);

var effect = new BattleStatusEffect(effectId, statusModel);
```

#### キャラクターへの効果適用
```csharp
// BattleStatusEffectModule での効果管理
var effectModule = new BattleStatusEffectModule();

// 効果追加
var updatedModule = effectModule.AddStatusEffect(effect);

// 効果削除
var removedModule = updatedModule.RemoveStatusEffect(effect);

// 現在の効果一覧取得
var currentEffects = effectModule.StatusEffects;
```

#### 継続効果処理
```csharp
// PassiveEffectService での継続効果実行
var service = new PassiveEffectService();

// 継続効果の非同期実行
await service.PlayContinuouslyEffect(characterId, continuousEffect);
```

### 未実装機能

**注意**：以下の機能はドキュメントに記載されていますが、現在未実装です：

#### 効果ファクトリーシステム
- **標準効果作成**：AttackBuff、DefenseBuff等の標準効果
- **カスタム効果**：ゲーム固有の特殊効果
- **効果テンプレート**：再利用可能な効果定義

#### 効果統合システム
- **複数効果合成**：複数の効果を統合した最終効果計算
- **戦闘パラメータ統合**：効果による最終戦闘パラメータ計算
- **効果相互作用**：効果間のシナジー・競合処理

#### 時間管理システム
- **持続時間**：ターンベースまたは時間ベースの持続時間
- **期限切れ処理**：自動的な効果削除
- **ターン処理**：戦闘ターン毎の効果更新

## 他ドメインとの連携

### Character ドメインとの連携

#### 実装済み連携
- **BattleStatusEffectModule**: Character ドメインで効果コレクション管理
- **CharacterID 参照**: 継続効果での対象キャラクター指定
- **戦闘ステータス補正**: MaxHealth、Attack、Defense、MovementSpeed の補正値提供

```csharp
// 実装済みの連携機能
public interface ContinuouslyEffect
{
    UniTask DoEffect(CharacterID targetId); // キャラクターID連携
}

public record BattleStatusEffectModule
{
    public IReadOnlyCollection<BattleStatusEffect> StatusEffects { get; }
    // Character ドメイン内での効果管理
}
```

#### 設計上の連携点
- **効果適用**: Character の戦闘パラメータへの効果反映
- **効果管理**: キャラクター固有の効果状態管理
- **非同期処理**: UniTask による継続効果の非同期実行

### 将来の連携可能性
> **⚠️ 要更新**: 以下は将来の拡張案であり、現在は未実装です
- **Item ドメイン**: 装備品・消耗品による効果付与
- **Skill ドメイン**: スキル使用による効果発動
- **Field ドメイン**: エリア固有の環境効果

## 拡張ポイント

### 実装可能な拡張

#### 持続時間システム
```csharp
// 将来実装可能：時間管理付き効果
public record TimedBattleStatusEffect : BattleStatusEffect
{
    public int Duration { get; }        // ターン数
    public float RemainingTime { get; } // 残り時間（秒）
    
    public TimedBattleStatusEffect DecreaseDuration(int turns = 1);
    public bool IsExpired => Duration <= 0;
}
```

#### 効果タイプシステム
```csharp
// 将来実装可能：効果分類システム
public enum EffectType
{
    Buff,           // 有利効果
    Debuff,         // 不利効果
    Neutral,        // 中性効果
    Environmental,  // 環境効果
}

public record CategorizedBattleStatusEffect : BattleStatusEffect
{
    public EffectType Type { get; }
    public bool IsStackable { get; }
}
```

#### 効果統合システム
```csharp
// 将来実装可能：複数効果の統合計算
public static class EffectAggregator
{
    public static BattleStatusEffectModel CombineEffects(
        IEnumerable<BattleStatusEffect> effects)
    {
        return effects.Aggregate(
            new BattleStatusEffectModel(0, 0, 0, 0),
            (acc, effect) => new BattleStatusEffectModel(
                acc.MaxHealth + effect.StatusModel.MaxHealth,
                acc.Attack + effect.StatusModel.Attack,
                acc.Defense + effect.StatusModel.Defense,
                acc.MovementSpeed + effect.StatusModel.MovementSpeed
            )
        );
    }
}
```

#### 効果ファクトリーシステム
```csharp
// 将来実装可能：標準効果作成
public static class EffectFactory
{
    public static BattleStatusEffect CreateAttackBuff(int bonus)
    {
        return new BattleStatusEffect(
            new EffectID(),
            new BattleStatusEffectModel(0, bonus, 0, 0)
        );
    }
    
    public static BattleStatusEffect CreateDefenseBuff(int bonus)
    {
        return new BattleStatusEffect(
            new EffectID(),
            new BattleStatusEffectModel(0, 0, bonus, 0)
        );
    }
}
```

### 設計基盤の特徴

現在の実装は将来の拡張に対応できる堅実な設計基盤を提供：

- **レコード型設計**: イミュータブルな効果管理
- **インターフェース分離**: 継続効果・瞬間効果の分離
- **Character 統合**: BattleStatusEffectModule による効果管理
- **非同期対応**: UniTask による効果処理
- **JSON 対応**: 完全なシリアライゼーション対応

PassiveEffect ドメインは Character ドメインとの密接な連携により、戦闘システムの効果管理基盤を提供しています。現在の実装は基本的な戦闘ステータス補正に特化していますが、将来の複雑な効果システムに対応できる柔軟な設計基盤を持っています。