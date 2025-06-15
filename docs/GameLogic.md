# BlackSmith.Domain ゲームロジック

## 概要

BlackSmith.Domain は Unity ベースのゲームプロジェクト用ドメインライブラリです。ドメイン駆動設計（DDD）の原則に従い、ドメインロジックと Unity 固有の実装を分離したクリーンアーキテクチャを採用しています。

## アーキテクチャ

### レイヤー構成

```
Assets/Plugins/BlackSmith.Domain/
├── Domain/          # ドメイン層（コアビジネスロジック）
├── Usecase/         # ユースケース層（アプリケーションロジック）
└── Tests/           # 単体テスト
```

### 設計パターン

- **値オブジェクト**: record型による不変オブジェクト
- **ファクトリーパターン**: エンティティ作成の責務分離
- **コマンドパターン**: エンティティ再構築の統一化
- **リポジトリパターン**: データアクセスの抽象化

## ドメイン構造

### 1. Character ドメイン

プレイヤーの基本情報、戦闘システム、レベル・経験値システムを管理します。

#### 主要コンポーネント

- **PlayerCommonEntity**: プレイヤーの基本情報
- **PlayerBattleEntity**: 戦闘時のプレイヤーエンティティ
- **Experience**: 経験値・レベル管理
- **LevelDependentParameters**: レベル依存パラメータ

#### 経験値システム

```csharp
// 指数関数的レベル上昇システム
// 必要経験値 = 初期値 * (1 - 倍率^(level-1)) / (1 - 倍率)
// 初期値: 100, 倍率: 1.25
```

#### ステータスシステム

- **Strength (STR)**: 筋力
- **Agility (AGI)**: 俊敏性
- レベル毎に3ポイントのステータス振り分け可能

#### 戦闘システム

```csharp
// 攻撃力計算
AttackValue = (STR + AGI) * 2 + WeaponAttack + ArmorAttack + StatusEffectAttack

// 防御力計算
DefenseValue = (STR + AGI) * 2 + WeaponDefense + ArmorDefense + StatusEffectDefense
```

### 2. Item ドメイン

アイテム、装備品、クラフトシステムを管理します。

#### 主要コンポーネント

- **Item**: 基本アイテムクラス
- **EquippableItem**: 装備可能アイテム
- **ICraftableItem**: クラフト可能アイテム
- **ICraftMaterialItem**: 素材アイテム

#### 装備システム

- **EquipmentType**: 装備種別
  - Weapon（武器）
  - Armor（防具）
  - Accessary（アクセサリ）

#### 強化システム

- **EnhancementParameter**: 強化パラメータ
  - 鋭さ（Sharpness）
  - 速さ（Speed）
  - 正確さ（Accuracy）
  - 重さ（Weight）
  - 丈夫さ（Durability）

### 3. Inventory ドメイン

インベントリ、通貨、取引システムを管理します。

#### インベントリシステム

- **EquipmentInventory**: 装備インベントリ（各部位1つまで）
- **InfiniteSlotInventory**: 無制限スロットインベントリ
- **ItemSlot**: アイテムスロット（アイテム + 個数管理）

#### 通貨システム

```csharp
public enum CurrencyType
{
    Sakura = 1,  // 桜式通貨
    Aren = 2,    // アレン式通貨
}
```

通貨間の両替機能を持つ Currency 値オブジェクトで管理されています。

### 4. Skill ドメイン

戦闘スキル、生産スキル、パッシブ効果を管理します。

#### スキル種別

- **BattleSkill**: 戦闘スキル
- **ProductionSkill**: 生産スキル
  - 作成（Creation）
  - 精錬（Refining）  
  - 修理（Repair）

#### スキル習得条件

- プレイヤーレベル
- 必要ステータス（STR/AGI）
- 前提スキル熟練度

#### スキル熟練度

- 範囲: 1-1000
- 経験値による段階的成長

### 5. Field ドメイン

4層構造の世界システムを管理します。

```
World（世界）
  └── Map（マップ）
      └── Field（フィールド）
          └── Chunk（チャンク）
```

- **World**: 最上位エリア（1桁数を想定）
- **Map**: 世界内の国、山、平原
- **Field**: マップ内の町、区画、ダンジョン（主要エリア単位）
- **Chunk**: フィールド内のレンダリング/処理分割

### 6. PassiveEffect ドメイン

ステータス効果、バフ・デバフシステムを管理します。

#### ステータス効果

```csharp
public record BattleStatusEffectModel
{
    public int MaxHealth { get; }
    public int Attack { get; }
    public int Defense { get; }
    public int MovementSpeed { get; }
}
```

複数のステータス効果を累積適用可能です。

### 7. Quest ドメイン

クエストシステム、依頼管理を行います。

- 依頼人情報
- 報酬設定
- 締切管理

## ビジネスルール

### キャラクターシステム

1. **経験値計算**: 指数関数的成長（倍率1.25）
2. **ステータス割り振り**: レベル毎に3ポイント
3. **体力計算**: レベル × 10の最大HP
4. **能力値下限**: すべての能力値は1以上

### 戦闘システム

1. **ダメージ計算**: 攻撃力 - 防御力（最低1ダメージ保証）
2. **装備制限**: 各部位1つまで装備可能
3. **ステータス効果**: 複数効果の累積適用

### アイテムシステム

1. **強化制限**: 強化回数に制限なし（各パラメータ個別管理）
2. **クラフト条件**: 必要素材の完全一致
3. **装備条件**: レベル・ステータス要件

### インベントリシステム

1. **スタック制限**: アイテム種別毎にスタック可能
2. **容量制限**: インベントリ種別により異なる
3. **装備インベントリ**: 各装備種別1つまで

## ユースケース層

### 主要ユースケース

#### キャラクター管理
- **PlayerCommonEntityProvideUsecase**: プレイヤーエンティティ提供
- **AdjustPlayerCommonEntityUsecase**: プレイヤー情報調整
- **RewritePlayerCommonEntityStatusUsecase**: ステータス更新

#### 戦闘システム
- **CharacterDamageUsecase**: ダメージ処理
- **AdjustPlayerBattleEntityUsecase**: 戦闘エンティティ調整
- **BattleEquipmentUsecase**: 装備変更処理

#### インベントリ管理
- **InventoryUseCase**: インベントリ操作
- **ItemCraftUsecase**: アイテムクラフト
- **ItemTradeUsecase**: アイテム取引

### リポジトリインターフェース

- **IPlayerCommonEntityRepository**: プレイヤー共通データ
- **IPlayerBattleEntityRepository**: プレイヤー戦闘データ
- **IInventoryRepository**: インベントリデータ

## 技術的特徴

### 関数型プログラミング要素

- Record型による不変オブジェクト
- メソッドチェーンによる状態変更
- 副作用の最小化

### 非同期処理

- UniTaskによるゲーム特化非同期処理
- リポジトリ操作の非同期化

### リアクティブプログラミング

- R3によるイベント駆動システム
- PlayerEventPublisherによるイベント配信

## 依存関係

### 外部ライブラリ

- **UniTask** (2.5.5): 非同期処理サポート
- **R3** (1.2.8): リアクティブプログラミング
- **Newtonsoft.Json** (3.2.1): JSONシリアライゼーション
- **System.Collections.Immutable** (9.0.0): イミュータブルコレクション

### ドメイン間依存関係

```
Character ←→ Item (装備システム)
Character ←→ Skill (スキル習得)
Character ←→ PassiveEffect (ステータス効果)
Character → Field (配置・移動)
Item ←→ Inventory (所有管理)
Currency ←→ Inventory (通貨管理)
Quest → Character (依頼人・対象)
```

循環依存はインターフェースによる依存性逆転、コマンドパターン、ファクトリーパターンで回避されています。